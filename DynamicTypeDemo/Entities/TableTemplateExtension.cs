using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Builders;
using DynamicTypeDemo.Entities;

namespace DynamicTypeDemo.Entities
{
    public static class TableTemplateExtension
    {
        public static Type CreateType(this TableTemplate template, string typeName, string tableName)
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
            typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable);
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute).GetConstructor(new Type[] { typeof(string)}),new object[] { tableName }));

            methodAttrs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

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

                else
                {
                    t = typeof(string);
                }
                fieldBuilder = typeBuilder.DefineField("field_" + field.Name, t, FieldAttributes.Private);
                propertyBuilder = typeBuilder.DefineProperty(field.Name, System.Reflection.PropertyAttributes.HasDefault, t, null);
                if (field.IsKey)
                {
                    propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.ComponentModel.DataAnnotations.KeyAttribute).GetConstructor(Type.EmptyTypes), new object[0]));//
                }

                if (propertyBuilder.PropertyType == typeof(string))
                {
                    propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.ComponentModel.DataAnnotations.MaxLengthAttribute).GetConstructor(new Type[] { typeof(int) }), new object[] { field.Length }));//
                }

                methodBuilder = typeBuilder.DefineMethod("get_" + field.Name, methodAttrs, t, Type.EmptyTypes);
                ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(methodBuilder);

                methodBuilder = typeBuilder.DefineMethod("set_" + field.Name, methodAttrs, typeof(void), new Type[] { t });
                ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetSetMethod(methodBuilder);

            }


            return typeBuilder.CreateType();
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
                if (field.Name == "sys_id")
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
