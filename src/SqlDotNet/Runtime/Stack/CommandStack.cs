using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Runtime stack. It executes all operations
    /// </summary>
    public class CommandStack
    {
        /// <summary>
        /// Global stack size
        /// </summary>
        public const ushort STACKSIZE = 4096;

        #region Private Member
        private StackItem[] slots;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the stack
        /// </summary>
        public CommandStack()
        {
            // Crate stack with static size
            slots = new StackItem[STACKSIZE];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Pop the value from the stack
        /// </summary>
        /// <returns></returns>
        internal StackItem Pop()
        {
            StackItem val = slots[0];

            for (int i = 0; i < (slots.Length - 1); i++)
            {
                slots[i] = slots[i + 1];
            }

            return val;
        }

        /// <summary>
        /// Peek value from command stack
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal StackItem Peek(ushort index)
        {
            return slots[index];
        }

        /// <summary>
        /// Push an object to the stack
        /// </summary>
        /// <param name="value">Value for calculation</param>
        public void Push(object value, DataType dataType)
        {
            // Push to the slots, the last item will be remove from the old stack
            for (ushort i = (STACKSIZE - 1); i > 0; i--)
            {
                slots[i] = slots[i - 1];
            }

            slots[0] = new StackItem() { Value = value, DataType = dataType };
        }

        /// <summary>
        /// Execute the stack
        /// </summary>
        /// <param name="opType">Task to execute</param>
        public void Execute(OperatorType opType)
        {
            var slot_0 = Pop();
            var slot_1 = Pop();

            dynamic slot1 = GetAsType(slot_0);
            dynamic slot2 = GetAsType(slot_1);

            DataType slot1DT = slot_0.DataType;
            DataType slot2DT = slot_1.DataType;
            DataType returnType = DataType.None;

            if (slot1DT >= slot2DT)
            {
                returnType = slot1DT;
            }
            else
            {
                returnType = slot2DT;
            }

            switch (opType)
            {
                case OperatorType.Add:
                    Push(slot2 + slot1, returnType);
                    break;

                case OperatorType.Sub:
                    if (slot1DT == DataType.Str || slot2DT == DataType.Str)
                    {
                        throw new Exception("Operator " + opType.ToString() + " not supported for strings");
                    }
                    else
                    {
                        Push(slot2 - slot1, returnType);
                    }
                    break;

                case OperatorType.Mul:
                    if (slot1DT == DataType.Str || slot2DT == DataType.Str)
                    {
                        throw new Exception("Operator " + opType.ToString() + " not supported for strings");
                    }
                    else
                    {
                        Push(slot2 * slot1, returnType);
                    }
                    break;

                case OperatorType.Div:
                    if (slot1DT == DataType.Str || slot2DT == DataType.Str)
                    {
                        throw new Exception("Operator " + opType.ToString() + " not supported for strings");
                    }
                    else
                    {
                        Push(slot2 / slot1, returnType);
                    }
                    break;
                case OperatorType.Equal:
                    Push(slot2 == slot1, DataType.Boolean);
                    break;
                case OperatorType.Unequal:
                    Push(slot2 != slot1, DataType.Boolean);
                    break;
                case OperatorType.Greater:
                    Push(slot2 > slot1, DataType.Boolean);
                    break;
                case OperatorType.Smaller:
                    Push(slot2 < slot1, DataType.Boolean);
                    break;
                case OperatorType.GreaterEqual:
                    Push(slot2 >= slot1, DataType.Boolean);
                    break;
                case OperatorType.SmallerEqual:
                    Push(slot2 <= slot1, DataType.Boolean);
                    break;
                case OperatorType.And:
                    Push(slot2 || slot1, DataType.Boolean);
                    break;
                case OperatorType.Or:
                    Push(slot2 && slot1, DataType.Boolean);
                    break;
            }
        }

        private dynamic GetAsType(StackItem item)
        {
            switch (item.DataType)
            {
                case DataType.Null:
                    return null;

                case DataType.Str:
                    return (string)item.Value;

                case DataType.Char:
                    return (char)item.Value;

                case DataType.Boolean:
                    return (bool)item.Value;

                case DataType.Int32:
                    return (int)item.Value;

                case DataType.Int64:
                    return (long)item.Value;

                case DataType.Float32:
                    return (float)item.Value;

                case DataType.Float64:
                    return (double)item.Value;
            }

            return null;
        }
        #endregion

        #region Public Member

        #endregion
    }
}
