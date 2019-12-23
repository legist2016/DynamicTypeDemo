using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicTypeDemo
{
    public enum DynamicConditionOperation
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None,
        /// <summary>
        /// ==
        /// </summary>
        [Description("==")]
        Eq,
        /// <summary>
        /// !=
        /// </summary>
        [Description("!=")]
        NE,
        /// <summary>
        /// >
        /// </summary>
        [Description(">")]
        Gt,
        /// <summary>
        /// <
        /// </summary>
        [Description("<")]
        Lt,
        /// <summary>
        /// >=
        /// </summary>
        [Description(">=")]
        GE,
        /// <summary>
        /// <=
        /// </summary>
        [Description("<=")]
        LE,
        /// <summary>
        /// 包含
        /// </summary>
        Ct,
        /// <summary>
        /// 开始是
        /// </summary>
        Sw,
        /// <summary>
        /// 结尾是
        /// </summary>
        Ew,
        /// <summary>
        /// 并且
        /// </summary>
        And,
        /// <summary>
        /// 或者
        /// </summary>
        Or
    }
    public class DynamicCondition
    {
        public DynamicConditionOperation Operation { get; set; }
        public string Field { get; set; }
        public object Value { get; set; }
        public List<DynamicCondition> Branchs { get; }
        public DynamicCondition()
        {
            Branchs = new List<DynamicCondition>();
            Operation = DynamicConditionOperation.None;
        }
        public DynamicCondition(string field, string operation, object value)
            :this(field, DynamicConditionOperation.None,value)
        {
            
            switch (operation)
            {
                case "==":
                    Operation = DynamicConditionOperation.Eq;
                    break;
                case "!=":
                    Operation = DynamicConditionOperation.NE;
                    break;
                case ">":
                    Operation = DynamicConditionOperation.Gt;
                    break;
                case "<":
                    Operation = DynamicConditionOperation.Lt;
                    break;
                case ">=":
                    Operation = DynamicConditionOperation.GE;
                    break;
                case "<=":
                    Operation = DynamicConditionOperation.LE;
                    break;
                case "%%":
                    Operation = DynamicConditionOperation.Ct;
                    break;
                case "%=":
                    Operation = DynamicConditionOperation.Sw;
                    break;
                case "=%":
                    Operation = DynamicConditionOperation.Ew;
                    break;
                default:
                    throw new Exception("无法识别的操作符号:"+ operation);
            }
            
        }
        public DynamicCondition(string field, DynamicConditionOperation operation, object value)
        {
            if(//operation == DynamicConditionOperation.None||
                operation==DynamicConditionOperation.And||
                operation == DynamicConditionOperation.Or)
            {
                throw new Exception("无效的条件操作类型");
            }
            Branchs = new List<DynamicCondition>();
            Operation = operation;
            Field = field;
            Value = value;
        }

        public override string ToString()
        {
            if (Operation == DynamicConditionOperation.None) return "";
            if (Operation == DynamicConditionOperation.And || Operation == DynamicConditionOperation.Or) {

                return string.Format("({0})",string.Join(string.Format(" {0} ", Operation), Branchs.ConvertAll<string>(p => p.ToString())));
            };
            if(Value.GetType() == typeof(string))
                return string.Format("({0} {1} \"{2}\")",Field, Operation, Value);
            return string.Format("({0} {1} {2})", Field, Operation, Value);
        }

        /// <summary>
        /// And
        /// </summary>
        /// <param name="condition1"></param>
        /// <param name="condition2"></param>
        /// <returns></returns>
        public static DynamicCondition operator*(DynamicCondition condition1, DynamicCondition condition2)
        {
            DynamicCondition condition3 = new DynamicCondition();
            condition3.Operation = DynamicConditionOperation.And;
            if(condition1.Operation== DynamicConditionOperation.And)
            {
                condition3.Branchs.AddRange(condition1.Branchs);
            }
            else
            {
                condition3.Branchs.Add(condition1);
            }
            if (condition2.Operation == DynamicConditionOperation.And)
            {
                condition3.Branchs.AddRange(condition2.Branchs);
            }
            else
            {
                condition3.Branchs.Add(condition2);
            }
            return condition3;
        }
        public static DynamicCondition operator +(DynamicCondition condition1, DynamicCondition condition2)
        {
            DynamicCondition condition3 = new DynamicCondition();
            condition3.Operation = DynamicConditionOperation.Or;
            if (condition1.Operation == DynamicConditionOperation.Or)
            {
                condition3.Branchs.AddRange(condition1.Branchs);
            }
            else
            {
                condition3.Branchs.Add(condition1);
            }
            if (condition2.Operation == DynamicConditionOperation.Or)
            {
                condition3.Branchs.AddRange(condition2.Branchs);
            }
            else
            {
                condition3.Branchs.Add(condition2);
            }
            return condition3;
        }

        static public DynamicCondition Parse(string condition)
        {
            try
            {
                int index = 1;
                Stack<string> strings = new Stack<string>();
                Dictionary<string, DynamicCondition> exps = new Dictionary<string, DynamicCondition>();
                var r = new Regex("\"(.*?[^\\\\])\"");
                condition = r.Replace(condition, p =>
                {
                    strings.Push(p.Groups[1].Value.Replace("\\\"", "\""));
                    return "$strings";
                });
                Console.WriteLine(condition);
                strings = new Stack<string>(strings);
                r = new Regex("([_a-zA-Z]\\w*?)\\s*([=!<>%]{1,2})\\s*([\\w\\.$]+)");
                var f = new Regex("\\d+\\.\\d+");
                var i = new Regex("\\d+");
                condition = r.Replace(condition, p =>
                {
                    //strings.Push(p.Value);
                    var field = p.Groups[1].Value;
                    var opertion = p.Groups[2].Value;
                    object value = p.Groups[3].Value;
                    if ((string)value == "$strings")
                    {
                        value = strings.Pop();
                    }
                    else if ((string)value == "true" || (string)value == "false")
                    {
                        value = bool.Parse((string)value);
                    }
                    else if (f.IsMatch((string)value))
                    {
                        value = float.Parse((string)value);
                    }
                    else if (i.IsMatch((string)value))
                    {
                        value = int.Parse((string)value);
                    }
                    else
                    {
                        throw new Exception("无效的数值格式。");
                    }
                    exps.Add("$exp" + index, new DynamicCondition(field, opertion, value));
                    return "$exp" + index++;
                });
                Console.WriteLine(condition);
                if (strings.Count > 0)
                {
                    throw new Exception("格式错误：" + strings.Pop());
                }

                r = new Regex("\\s*\\(\\s*(\\$exp\\d+)\\s*\\)\\s*");
                var and = new Regex("(\\$exp\\d+)\\s*and\\s*(\\$exp\\d+)", RegexOptions.IgnoreCase);
                var or = new Regex("(\\$exp\\d+)\\s*or\\s*(\\$exp\\d+)", RegexOptions.IgnoreCase);
                do
                {

                    do
                        condition = and.Replace(condition, p =>
                        {
                            exps.Add("$exp" + index, exps[p.Groups[1].Value] * exps[p.Groups[2].Value]);
                            return string.Format(" $exp{0} ", index++);
                        });
                    while (and.IsMatch(condition));
                    Console.WriteLine(condition);
                    do
                        condition = or.Replace(condition, p =>
                        {
                            exps.Add("$exp" + index, exps[p.Groups[1].Value] + exps[p.Groups[2].Value]);
                            return string.Format(" $exp{0} ", index++);
                        });
                    while (or.IsMatch(condition));
                    Console.WriteLine(condition);
                    condition = r.Replace(condition, p =>
                    {
                        return " " + p.Groups[1].Value + " ";

                    });
                    Console.WriteLine(condition);
                } while (r.IsMatch(condition) || and.IsMatch(condition) || or.IsMatch(condition));
                Console.WriteLine(condition);
                return exps[condition.Trim()];
            }
            catch
            {
                throw;
            }

        }

    }
    static class DynamicConditionExtension
    {
        static public DynamicCondition And(this DynamicCondition condition, DynamicCondition condition2)
        {
            return condition * condition2;
        }
        public static DynamicCondition And(this DynamicCondition condition, string field, DynamicConditionOperation operation, object value)
        {
            return condition * new DynamicCondition(field,operation,value);
        }
        public static DynamicCondition And(this DynamicCondition condition, string field, string operation, object value)
        {
            return condition * new DynamicCondition(field, operation, value);
        }
        static public DynamicCondition Or(this DynamicCondition condition, DynamicCondition condition2)
        {
            return condition + condition2;
        }
        public static DynamicCondition Or(this DynamicCondition condition, string field, DynamicConditionOperation operation, object value)
        {
            return condition + new DynamicCondition(field, operation, value);
        }
        public static DynamicCondition Or(this DynamicCondition condition, string field, string operation, object value)
        {
            return condition + new DynamicCondition(field, operation, value);
        }

        static public Expression<Func<T, bool>> GenerateExperssion<T>(this DynamicCondition condition)
        {
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, bool>>(condition.GenerateExperssion(param),param);
        }

        static public Expression GenerateExperssion(this DynamicCondition condition, Expression param = null)
        {
            /*var param = Expression.Parameter(typeof(T));
            var property = Expression.PropertyOrField(param, field);
            var body = Expression.Equal(property, Expression.Constant(value, typeof(V)));
            return Expression.Lambda<Func<T, bool>>(body, param);*/
            Expression body = null;
            Expression left = null;
            if (param == null)
            {
                
            }
            if (condition.Operation == DynamicConditionOperation.And)
            {
                
                foreach (var branch in condition.Branchs)
                {
                    if(left == null)
                    {
                        left = branch.GenerateExperssion(param);
                    }else if (body == null)
                    {
                        body = Expression.AndAlso(left, branch.GenerateExperssion(param));
                    }
                    else
                    {
                        body = Expression.AndAlso(body, branch.GenerateExperssion(param));
                    }
                    
                }
            }
            else if (condition.Operation == DynamicConditionOperation.Or)
            {
                foreach (var branch in condition.Branchs)
                {
                    if (left == null)
                    {
                        left = branch.GenerateExperssion(param);
                    }
                    else if (body == null)
                    {
                        body = Expression.OrElse(left, branch.GenerateExperssion(param));
                    }
                    else
                    {
                        body = Expression.OrElse(body, branch.GenerateExperssion(param));
                    }

                }
            }
            else
            {
                var property = Expression.PropertyOrField(param, condition.Field);
                switch (condition.Operation)
                {
                    case DynamicConditionOperation.Eq:
                        body = Expression.Equal(property, Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.NE:
                        body = Expression.NotEqual(property, Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.Gt:
                        body = Expression.GreaterThan(property, Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.GE:
                        body = Expression.GreaterThanOrEqual(property, Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.Lt:
                        body = Expression.LessThan(property, Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.LE:
                        body = Expression.LessThanOrEqual(property, Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.Ct:
                        //Expression<Func<String, bool>> exp = s => s.Contains((string)condition.Value);


                        body = Expression.Call(
                            property,   
                            typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                            Expression.Constant(condition.Value));
                        
                        break;
                    case DynamicConditionOperation.Sw:
                        body = Expression.Call(
                            property,
                            typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
                            Expression.Constant(condition.Value));
                        break;
                    case DynamicConditionOperation.Ew:
                        body = Expression.Call(
                            property,
                            typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }),
                            Expression.Constant(condition.Value));
                        break;
                    default:
                        throw new Exception();

                }

            }
            return body;
        }
    }
}
