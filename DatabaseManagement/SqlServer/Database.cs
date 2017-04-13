using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public class Database : IDatabase
    {
        public string ConnectionString { get; private set; }

        public string Name { get; private set; }

        public string ConnectionStringName { get; private set; }

        public int DatabaseId { get; private set; }

        public Schema DefaultSchema { get; private set; }

        public IReadOnlyCollection<Schema> Schemas { get; private set; }

        public IReadOnlyCollection<Table> Tables { get; private set; }

        public IReadOnlyCollection<View> Views { get; private set; }

        public IReadOnlyCollection<UniqueConstraint> UniqueConstraints { get; private set; }

        public IReadOnlyCollection<ForeignKeyConstraint> ForeignKeys { get; private set; }

        private string _dbNameLower;

        public SchemaObject GetObjectById(int id)
        {
            SchemaObject ret;
            if (_objectsIndexedById.TryGetValue(id, out ret))
            {
                return ret;
            }
            else
            {
                throw new ArgumentException("No object in the database with id " + id + ".");
            }
        }
        public Schema GetSchemaByName(string name)
        {
            Schema ret;
            if (_schemasIndexedByName.TryGetValue(name.TrimBracketNotation().ToLower(), out ret))
            {
                return ret;
            }
            else
            {
                throw new ArgumentException("No schema \"" + name + "\" exists in database " + this + ".");
            }
        }

        public SchemaObject GetObjectByName(string name)
        {
            SchemaObject ret = GetObjectOrNull(name);
            /*if (ret == null)
            {
                throw new ArgumentException("No object \"" + name + "\" exists in database " + this + ".", "name");
            }*/
            return ret;
        }

        internal Schema GetSchemaOrNull(string name)
        {
            Schema ret;
            _schemasIndexedByName.TryGetValue(name.TrimBracketNotation().ToLowerInvariant(), out ret);
            return ret;
        }

        internal SchemaObject GetObjectOrNull(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string[] splitIntoParts = name.SplitQualifiedNameIntoParts();
            Schema schema;
            string objectName;
            switch (splitIntoParts.Length)
            {
                case 1:
                    schema = DefaultSchema;
                    objectName = splitIntoParts[0];
                    break;
                case 2:
                    schema = GetSchemaOrNull(splitIntoParts[0]);
                    objectName = splitIntoParts[1];
                    break;
                case 3:
                    string dbName = splitIntoParts[0];
                    if (dbName != _dbNameLower)
                    {
                        return null;
                    }
                    schema = GetSchemaOrNull(splitIntoParts[1]);
                    objectName = splitIntoParts[2];
                    break;
                default:
                    return null;
            }
            if (schema == null)
            {
                return null;
            }
            else
            {
                return schema.GetObjectOrNull(objectName);
            }
        }

        ISchemaObject IDatabase.GetObjectByName(string name)
        {
            return GetObjectByName(name);
        }

        IReadOnlyCollection<ITable> IDatabase.Tables
        {
            get { return Tables; }
        }

        IReadOnlyCollection<IView> IDatabase.Views
        {
            get { return Views; }
        }

        IReadOnlyCollection<IForeignKeyConstraint> IDatabase.ForeignKeys
        {
            get { return ForeignKeys; }
        }

        IReadOnlyCollection<IUniqueConstraint> IDatabase.UniqueConstraints
        {
            get { return UniqueConstraints; }
        }

        private Dictionary<int, SchemaObject> _objectsIndexedById;
        private Dictionary<int, Schema> _schemasIndexedById;
        private Dictionary<string, Schema> _schemasIndexedByName;

        internal Database(string connectionStringName, string connectionString)
        {
            ConnectionStringName = connectionStringName;
            ConnectionString = connectionString;
            mapDatabase();
        }

        private void mapDatabase()
        {
            _objectsIndexedById = new Dictionary<int, SchemaObject>();
            using (DataSet schemaInfo = new DataSet())
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand { Connection = conn })
                    {
                        cmd.CommandText = string.Join("\n",
                            MappingQueries.DB_INFO_QUERY,
                            MappingQueries.ALL_SCHEMAS_QUERY,
                            MappingQueries.ALL_TABLES_QUERY,
                            MappingQueries.ALL_VIEWS_QUERY,
                            MappingQueries.ALL_UNIQUE_CONSTRAINTS_QUERY,
                            MappingQueries.ALL_FOREIGN_KEY_CONSTRAINTS_QUERY,
                            MappingQueries.ALL_COLUMNS_QUERY,
                            MappingQueries.FOREIGN_KEY_COLUMN_MAPPING_QUERY,
                            MappingQueries.UNIQUE_CONSTRAINT_COLUMN_MAPPING_QUERY);
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(schemaInfo);
                        }
                    }
                    DataTableCollection tables = schemaInfo.Tables;
                    DataTable
                        dt_dbInfo = tables[0],
                        dt_allSchemas = tables[1],
                        dt_allTables = tables[2],
                        dt_allViews = tables[3],
                        dt_allUniqueConstraints = tables[4],
                        dt_allForeignKeyConstraints = tables[5],
                        dt_allColumns = tables[6],
                        dt_fkColMapping = tables[7],
                        dt_uniqueConstraintColMapping = tables[8];
                    List<Schema> allSchemas = new List<Schema>();
                    foreach (DataRow row in dt_allSchemas.Rows)
                    {
                        allSchemas.Add(new Schema
                        {
                            Id = Convert.ToInt32(row["schema_id"]),
                            Name = (string)row["name"],
                            Database = this
                        });
                    }
                    _schemasIndexedById = allSchemas.ToDictionary(s => s.Id);
                    _schemasIndexedByName = allSchemas.ToDictionary(s => s.Name);
                    this.Schemas = new GenericCollection<Schema>(allSchemas);
                    List<Table> allTables = new List<Table>();
                    foreach (DataRow row in dt_allTables.Rows)
                    {
                        Table t = new Table();
                        initObjectProperties(t, row);
                        allTables.Add(t);
                    }
                    this.Tables = new GenericCollection<Table>(allTables);
                    List<View> allViews = new List<View>();
                    foreach (DataRow row in dt_allViews.Rows)
                    {
                        View v = new View();
                        initObjectProperties(v, row);
                        v.CheckOption = (ViewCheckOption)Convert.ToInt32(row["with_check_option"]);
                        allViews.Add(v);
                    }
                    this.Views = new GenericCollection<View>(allViews);
                    List<Column> allColumns = new List<Column>();
                    foreach (DataRow row in dt_allColumns.Rows)
                    {
                        allColumns.Add(new Column
                        {
                            Name = (string)row["name"],
                            Table = (Table)GetObjectById(Convert.ToInt32(row["object_id"])),
                            OrdinalPosition = Convert.ToInt32(row["ordinal_position"]),
                            DefaultValue = row.Field<string>("column_default"),
                            IsNullable = Convert.ToBoolean(row["is_nullable"]),
                            DBType = parseDbType((string)row["data_type"]),
                            IsIdentity = Convert.ToBoolean(row["is_identity"]),
                            NumericPrecision = Convert.ToInt32(row["precision"]),
                            NumericScale = Convert.ToInt32(row["scale"]),
                            CharacterMaxLength = Convert.ToInt32(row["max_length"])
                        });
                    }
                    List<UniqueConstraint> allUniqueConstraints = new List<UniqueConstraint>();
                    foreach (DataRow row in dt_allUniqueConstraints.Rows)
                    {
                        UniqueConstraint uc = new UniqueConstraint();
                        initObjectProperties(uc, row);
                        uc.Table = (Table)GetObjectById(Convert.ToInt32(row["parent_object_id"]));
                        uc.IsPrimaryKey = (string)row["type"] == "PK";
                        if (uc.IsPrimaryKey)
                        {
                            if (uc.Table.PrimaryKey != null)
                            {
                                throw new DatabaseIntegrityException("Detected multiple primary keys on table " + uc.Table + ".");
                            }
                            uc.Table.PrimaryKey = uc;
                        }
                        allUniqueConstraints.Add(uc);
                    }
                    this.UniqueConstraints = new GenericCollection<UniqueConstraint>(allUniqueConstraints);
                    List<ForeignKeyConstraint> allForeignKeys = new List<ForeignKeyConstraint>();
                    foreach (DataRow row in dt_allForeignKeyConstraints.Rows)
                    {
                        ForeignKeyConstraint fk = new ForeignKeyConstraint();
                        initObjectProperties(fk, row);
                        fk.Table = (Table)GetObjectById(Convert.ToInt32(row["parent_object_id"]));
                        fk.ReferencedTable = (Table)GetObjectById(Convert.ToInt32(row["referenced_object_id"]));
                        fk.DeleteRule = (ForeignKeyRule)Convert.ToInt32(row["delete_referential_action"]);
                        fk.UpdateRule = (ForeignKeyRule)Convert.ToInt32(row["update_referential_action"]);
                        allForeignKeys.Add(fk);
                    }
                    this.ForeignKeys = new GenericCollection<ForeignKeyConstraint>(allForeignKeys);
                    foreach (Table t in allTables)
                    {
                        t.UniqueConstraints = new GenericCollection<UniqueConstraint>(allUniqueConstraints.Where(u => u.Table == t));
                        t.ForeignKeys = new GenericCollection<ForeignKeyConstraint>(allForeignKeys.Where(f => f.Table == t));
                        t.Constraints = new GenericCollection<TableConstraint>(t.ForeignKeys.Concat<TableConstraint>(t.UniqueConstraints));
                        t.Columns = new TableColumnCollection(t, allColumns.Where(c => c.Table == t).OrderBy(c => c.OrdinalPosition));
                        t.IdentityColumn = t.Columns.SingleOrDefault(c => c.IsIdentity);
                    }
                    List<ForeignKeyColumnMapping> allFkColMappings = new List<ForeignKeyColumnMapping>();
                    foreach (DataRow row in dt_fkColMapping.Rows)
                    {
                        ForeignKeyConstraint fk = (ForeignKeyConstraint)GetObjectById(Convert.ToInt32(row["constraint_object_id"]));
                        allFkColMappings.Add(new ForeignKeyColumnMapping
                        {
                            Constraint = fk,
                            Position = Convert.ToInt32(row["constraint_column_id"]),
                            ForeignKeyColumn = fk.Table.Columns[(string)row["parent_column_name"]],
                            ReferencedColumn = fk.ReferencedTable.Columns[(string)row["referenced_column_name"]]
                        });
                    }
                    List<UniqueConstraintColumnMapping> allUcColMappings = new List<UniqueConstraintColumnMapping>();
                    foreach (DataRow row in dt_uniqueConstraintColMapping.Rows)
                    {
                        UniqueConstraint u = (UniqueConstraint)GetObjectById(Convert.ToInt32(row["object_id"]));
                        allUcColMappings.Add(new UniqueConstraintColumnMapping
                        {
                            Constraint = u,
                            Position = Convert.ToInt32(row["index_column_id"]),
                            Column = u.Table.Columns[(string)row["name"]]
                        });
                    }
                    foreach (Schema s in allSchemas)
                    {
                        s.Tables = new GenericCollection<Table>(allTables.Where(t => t.Schema == s));
                        s.UniqueConstraints = new GenericCollection<UniqueConstraint>(allUniqueConstraints.Where(u => u.Schema == s));
                        s.ForeignKeys = new GenericCollection<ForeignKeyConstraint>(allForeignKeys.Where(f => f.Schema == s));
                        s.AllObjects = new GenericCollection<SchemaObject>(s.Tables.Union<SchemaObject>(s.UniqueConstraints).Union(s.ForeignKeys));
                        s._nameIndexed = s.AllObjects.ToDictionary(o => o.Name.ToLowerInvariant());
                    }
                    foreach (ForeignKeyConstraint fk in allForeignKeys)
                    {
                        ForeignKeyColumnMapping[] children = allFkColMappings.Where(m => m.Constraint == fk).OrderBy(m => m.Position).ToArray();
                        fk.ForeignKeyColumns = new GenericCollection<Column>(children.Select(m => m.ForeignKeyColumn));
                        fk.ReferencedColumns = new GenericCollection<Column>(children.Select(m => m.ReferencedColumn));
                    }
                    foreach (UniqueConstraint u in allUniqueConstraints)
                    {
                        u.ConstrainedColumns = new GenericCollection<Column>(allUcColMappings.Where(m => m.Constraint == u).OrderBy(m => m.Position).Select(m => m.Column));
                    }
                    DataRow dbInfoRow = dt_dbInfo.Rows.Cast<DataRow>().Single();
                    this.DatabaseId = Convert.ToInt32(dbInfoRow["DB_ID"]);
                    this.Name = (string)dbInfoRow["DB_NAME"];
                    this._dbNameLower = this.Name.ToLowerInvariant();
                    int defaultSchemaId = Convert.ToInt32(dbInfoRow["DEFAULT_SCHEMA_ID"]);
                    this.DefaultSchema = this.Schemas.First(s => s.Id == defaultSchemaId);
                }
            }
        }

        private void initObjectProperties(SchemaObject obj, DataRow row)
        {
            obj.Name = (string)row["name"];
            obj.ObjectId = Convert.ToInt32(row["object_id"]);
            obj.Schema = _schemasIndexedById[Convert.ToInt32(row["schema_id"])];
            obj.CreatedDate = (DateTime)row["create_date"];
            obj.ModifyDate = (DateTime)row["modify_date"];
            _objectsIndexedById.Add(obj.ObjectId, obj);
        }

        static SqlDbType parseDbType(string s)
        {
            SqlDbType ret;
            if (!Enum.TryParse<SqlDbType>(s, true, out ret))
            {
                ret = SqlDbType.Udt;
            }
            return ret;
        }

        IDbConnection IDatabase.GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /*public void ExecuteCommand(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = commandType;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteCommandGetScalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = commandType;
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataTable ExecuteCommandGetDataTable(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = commandType;
                    DataTable ret = new DataTable();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ret.Load(reader);
                    }
                    return ret;
                }
            }
        }*/

        public override string ToString()
        {
            return "[" + Name + "]";
        }
    }
}
