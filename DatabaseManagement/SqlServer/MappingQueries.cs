using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    internal static class MappingQueries
    {
        internal const string DB_INFO_QUERY =
@"select
DB_NAME() DB_NAME,
DB_ID() DB_ID,
SCHEMA_ID(SCHEMA_NAME()) DEFAULT_SCHEMA_ID";

        internal const string ALL_TABLES_QUERY =
@"select
object_id,
name,
schema_id,
create_date,
modify_date,
type
from sys.objects
where type = 'U'";

        internal const string ALL_SCHEMAS_QUERY =
@"select
schema_id,
name
from sys.schemas";

        internal const string ALL_VIEWS_QUERY =
@"select
object_id,
name,
schema_id,
create_date,
modify_date,
with_check_option
from sys.views";

        internal const string ALL_UNIQUE_CONSTRAINTS_QUERY =
@"select name,
object_id,
schema_id,
parent_object_id,
type,
create_date,
modify_date
from sys.key_constraints";

        internal const string ALL_FOREIGN_KEY_CONSTRAINTS_QUERY =
@"select
name,
object_id,
schema_id,
create_date,
modify_date,
parent_object_id,
referenced_object_id,
delete_referential_action,
update_referential_action
from sys.foreign_keys";

        internal const string ALL_COLUMNS_QUERY =
@"select
c.name,
c.object_id,
COLUMNPROPERTY(c.object_id, c.name, 'ordinal') ordinal_position,
convert(nvarchar(4000), OBJECT_DEFINITION(c.default_object_id)) column_default,
c.is_nullable,
ISNULL(TYPE_NAME(c.system_type_id), t.name) data_type,
c.is_identity,
c.precision,
c.scale,
ISNULL(COLUMNPROPERTY(c.object_id, c.name, 'charmaxlen'), 0) max_length
from sys.columns c
inner join sys.all_objects o on o.object_id = c.object_id and o.type in ('U','V')
LEFT JOIN sys.types t ON c.user_type_id = t.user_type_id
order by c.object_id, ordinal_position";

        internal const string FOREIGN_KEY_COLUMN_MAPPING_QUERY =
@"select
f.constraint_object_id,
f.constraint_column_id,
pc.name parent_column_name,
rc.name referenced_column_name
from sys.foreign_key_columns f
inner join sys.columns pc on pc.object_id = f.parent_object_id and pc.column_id = f.parent_column_id
inner join sys.columns rc on rc.object_id = f.referenced_object_id and rc.column_id = f.referenced_column_id
order by f.constraint_column_id";

        internal const string UNIQUE_CONSTRAINT_COLUMN_MAPPING_QUERY =
@"select
c.name,
ic.index_column_id,
k.object_id
from
sys.key_constraints k JOIN sys.index_columns ic
ON ic.object_id = k.parent_object_id AND ic.index_id = k.unique_index_id
join sys.columns c on c.object_id = ic.object_id and ic.index_column_id = c.column_id";
    }
}