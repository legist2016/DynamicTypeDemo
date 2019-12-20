using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Builders;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeDemo.Entities
{
    public static class TableTemplateExtension
    {
        public static Type CreateType(this TableTemplate template, string typeName, Type parent = null, Type[] interfaces = null)
        {
            return CreateType(template, typeName, typeName, parent,interfaces);
        }
        public static Type CreateType(this TableTemplate template, string typeName , string tableName,Type parent = null, Type[] interfaces =null )
        {
            //应用程序域
            AppDomain currentDomain = System.Threading.Thread.GetDomain(); //AppDomain.CurrentDomain;
            //运行并创建类的新实例
            TypeBuilder typeBuilder = null;
            //定义和表示动态程序集的模块
            ModuleBuilder moduleBuilder = null;
            MethodBuilder methodBuilder = null;
            //表示类型属性
            PropertyBuilder propertyBuilder = null;
            //定义表示字段
            FieldBuilder fieldBuilder = null;
            //定义表示动态程序集
            AssemblyBuilder assemblyBuilder = null;
            //生成中间语言指令
            ILGenerator ilGenerator = null;
            //帮助生成自定义属性
            CustomAttributeBuilder cab = null;
            //指定方法属性的标志
            MethodAttributes methodAttrs;

            //Define a Dynamic Assembly
            //指定名称，访问模式
            assemblyBuilder = currentDomain.DefineDynamicAssembly(new AssemblyName("TableTemplate"), AssemblyBuilderAccess.Run);


            //Define a Dynamic Module动态模块名称
            moduleBuilder = assemblyBuilder.DefineDynamicModule(string.Format("Module{0}", typeName), true);

            //Define a runtime class with specified name and attributes.
            
            typeBuilder = moduleBuilder.DefineType(
                typeName, 
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable,
                parent,
                interfaces
                );
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute).GetConstructor(new Type[] { typeof(string)}),new object[] { tableName }));

            foreach (var type in interfaces)
            {
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    int length = 0;
                    var attribute = property.GetCustomAttribute(typeof(MaxLengthAttribute)) as MaxLengthAttribute;
                    if (attribute != null)
                    {
                        length = attribute.Length;
                    }
                    typeBuilder.CreateProperty(property.Name, property.PropertyType, length, property.IsDefined(typeof(KeyAttribute)), true);
                }
            }

            foreach (var field in template.Fields)
            {
                Type t;
                if (field.Type == TableTemplateFieldType.Int)
                {
                    t = typeof(int);
                }
                else if (field.Type == TableTemplateFieldType.String)
                {
                    t = typeof(string);
                }
                else if (field.Type == TableTemplateFieldType.Boolean)
                {
                    t = typeof(bool);
                }
                else
                {
                    t = typeof(string);
                }
                typeBuilder.CreateProperty(field.Name, t, field.Length);

            }


            return typeBuilder.CreateType();
        }

        static void CreateProperty(this TypeBuilder typeBuilder, string name, Type t, int Length ,  bool isKey = false, bool isVirtual = false)
        {
            var fieldBuilder = typeBuilder.DefineField("field_" + name, t, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(name, System.Reflection.PropertyAttributes.HasDefault, t, null);
            var methodAttrs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            if(isVirtual) {
                methodAttrs |= MethodAttributes.Virtual;
            }

            if (isKey)
            {
                propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.ComponentModel.DataAnnotations.KeyAttribute).GetConstructor(Type.EmptyTypes), new object[0]));
            }

            if (propertyBuilder.PropertyType == typeof(string) && Length > 0)
            {
                propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.ComponentModel.DataAnnotations.MaxLengthAttribute).GetConstructor(new Type[] { typeof(int) }), new object[] { Length }));//
            }

            var methodBuilder = typeBuilder.DefineMethod("get_" + name, methodAttrs, t, Type.EmptyTypes);
            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(methodBuilder);

            methodBuilder = typeBuilder.DefineMethod("set_" + name, methodAttrs, typeof(void), new Type[] { t });
            ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(methodBuilder);
        }



        public static string SQLGenerateCreateTable(this Type type, string tableName)
        {
            SqlServerMigrationSqlGenerator gen = new SqlServerMigrationSqlGenerator();
            ColumnBuilder columnBuilder = new ColumnBuilder();
            var properties = type.GetProperties();
            var operations = new List<MigrationOperation>();
            var table = new CreateTableOperation(tableName);
            table.PrimaryKey = new AddPrimaryKeyOperation();

            foreach(var property in properties)
            {
                PrimitiveTypeKind typeKind;
                if (property.PropertyType == typeof(int))
                {
                    typeKind = PrimitiveTypeKind.Int32;
                }
                else if (property.PropertyType == typeof(string))
                {
                    typeKind = PrimitiveTypeKind.String;
                }
                else if (property.PropertyType == typeof(bool))
                {
                    typeKind = PrimitiveTypeKind.Boolean;
                }
                else
                {
                    //typeKind = PrimitiveTypeKind.String;
                    continue;
                }
                var column = new ColumnModel(typeKind);
                column.Name = property.Name;
                if (column.Type == PrimitiveTypeKind.String)
                {
                    var attribute = property.GetCustomAttribute(typeof(MaxLengthAttribute)) as MaxLengthAttribute;
                    if (attribute != null)
                    {
                        column.MaxLength = attribute.Length;
                    }
                }
                if (property.IsDefined(typeof(KeyAttribute)))
                {
                    table.PrimaryKey.Columns.Add(property.Name);
                    if (column.Type == PrimitiveTypeKind.Int32)
                    {
                        column.IsIdentity = true;
                    }
                }
                table.Columns.Add(column);

            }
            operations.Add(table);
            var sql = gen.Generate(operations, "2008").FirstOrDefault();

            return sql.Sql;
        }
        public static string SQLGenerateAlterTable(this TableTemplate template, string tableName)
        {
            SqlServerMigrationSqlGenerator gen = new SqlServerMigrationSqlGenerator();
            ColumnBuilder columnBuilder = new ColumnBuilder();
            //columnBuilder.Int(name:"sysid",identity:true);

            //System.Data.Entity.Migrations.DbMigration
            var operations = new List<MigrationOperation>();
            var dict = new Dictionary<string, AnnotationValues>();
            dict.Add("dddddd", new AnnotationValues(1, 2));
            var alter = new AlterTableOperation("dddd", dict);


            //alter.Columns.Add(new ColumnModel(PrimitiveTypeKind.String) { Name="dd" });


            //operations.Add(alter);
            operations.Add(new AddColumnOperation(tableName, columnBuilder.String(name:"dd")));
            operations.Add(new AlterColumnOperation(tableName, new ColumnModel(PrimitiveTypeKind.String) { Name = "dd3", MaxLength = 20 }, false));
            operations.Add(new AlterColumnOperation(tableName, new ColumnModel(PrimitiveTypeKind.String) { Name = "dd2", MaxLength = 50 }, false));
            operations.Add(new RenameColumnOperation(tableName, "dd3", "ee3"));

            var sqls = gen.Generate(operations, "2008").Select<MigrationStatement, string>(ms => ms.Sql).ToArray();
            var sql = string.Join("\r\n\r\n", sqls);
            return sql;
        }

        public static string SQLGenerateCreateTable(this TableTemplate template, string tableName)
        {
            SqlServerMigrationSqlGenerator gen = new SqlServerMigrationSqlGenerator();

            var operations = new List<MigrationOperation>();

            var table = new CreateTableOperation(tableName);
            table.PrimaryKey = new AddPrimaryKeyOperation();
            foreach (var field in template.Fields)
            {
                PrimitiveTypeKind typeKind;
                if (field.Type == TableTemplateFieldType.Int)
                {
                    typeKind = PrimitiveTypeKind.Int32;
                }
                else if (field.Type == TableTemplateFieldType.String)
                {
                    typeKind = PrimitiveTypeKind.String;
                }
                else
                {
                    typeKind = PrimitiveTypeKind.String;
                }
                var column = new ColumnModel(typeKind);
                column.Name = field.Name;
                if(column.Type== PrimitiveTypeKind.String)
                {
                    column.MaxLength = field.Length;
                }
                if (field.IsKey)
                {                    
                    table.PrimaryKey.Columns.Add(field.Name);
                }
                if (field.Name.ToLower() == "sys_id")
                {
                    column.IsIdentity = true;
                }
                table.Columns.Add(column);
            }
            operations.Add(table);
            var sql = gen.Generate(operations, "2008").FirstOrDefault();
            
            return sql.Sql;
        }
    }
}
