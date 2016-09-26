using Simplic.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Creates the syntax-tree from a list of taw tokens
    /// </summary>
    public class SyntaxTreeBuilder
    {
        #region Private Member
        private Dequeue<RawToken> tokens;
        private ParserConfiguration parserConfig;
        private IErrorListener errorListener;
        private Stack<IScopeNode> scopeNodes;
        private EntryPointNode entryPoint;
        #endregion

        #region Constructor
        /// <summary>
        /// Create syntax tree builder
        /// </summary>
        public SyntaxTreeBuilder(ParserConfiguration parserConfig, Dequeue<RawToken> tokens, IErrorListener errorListener)
        {
            this.tokens = tokens;
            this.parserConfig = parserConfig;
            this.errorListener = errorListener;
            this.scopeNodes = new Stack<IScopeNode>();
        }
        #endregion

        #region Public Methods

        #region [Build]
        /// <summary>
        /// Create the syntax tree
        /// </summary>
        /// <param name="codeFragment">Fragment to build. This is important for the entry point of the script part</param>
        public EntryPointNode Build()
        {
            var processor = new TokenPreProcessor();
            tokens = processor.Process(tokens, parserConfig, errorListener);

            // Create entry-point
            this.entryPoint = new EntryPointNode();
            this.scopeNodes.Push(this.entryPoint);

            // Create entry-point
            this.scopeNodes.Push(entryPoint);

            // Parse all tokens
            while (tokens.Count > 0)
            {
                // Create syntaxtreenode from tokens
                Parse(this.entryPoint, tokens, entryPoint.SymbolTable);
            }

            return entryPoint;
        }
        #endregion

        /// <summary>
        /// Create syntax tree
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tokens"></param>
        /// <param name="symbolTable"></param>
        /// <returns></returns>
        internal SyntaxTreeNode Parse(SyntaxTreeNode parent, Dequeue<RawToken> tokens, SymbolTable symbolTable)
        {
            IList<SyntaxTreeNode> treeNodeList = new List<SyntaxTreeNode>();
            SyntaxTreeNode returnNode = parent;

            RawToken lastToken = null;
            try
            {

                while (tokens.Count > 0)
                {
                    RawToken token = tokens.PopFirst();
                    bool exitLoop = false;

                    switch (token.Type)
                    {
                        #region [Ignore tokens]
                        case TokenType.NewLine:
                            break;
                        #endregion

                        #region [TokenType.Select]
                        case TokenType.Select:
                            {
                                // Create new select node
                                returnNode = parent.CreateChildNode<SelectNode>(token, symbolTable);

                                // Pre cache select nodes
                                entryPoint.SelectNodes.Add((SelectNode)returnNode);

                                // Parse [COLUMNS/*] return column list until another keyword arrives
                                var list = ParseExpressionList(returnNode, tokens, ((SelectNode)returnNode).SymbolTable, true, new[] { TokenType.From, TokenType.Where }); // Add new cancel tokens if implemented
                                if (list.Children.Count == 0)
                                {
                                    ReportError("S5421", "Select must contain at least a minimum of one return-column.", token);
                                }

                                // Parse [FROM]
                                if (tokens.Count > 0 && tokens.PeekFirst().Type == TokenType.From)
                                {
                                    var from = Parse(returnNode, tokens, ((SelectNode)returnNode).SymbolTable);
                                }

                                // Parse [WHERE]
                                if (tokens.Count > 0 && tokens.PeekFirst().Type == TokenType.Where)
                                {
                                    var where = Parse(returnNode, tokens, ((SelectNode)returnNode).SymbolTable);
                                }

                                // Parse [GROUP-BY]
                                // - Not yet

                                // Parse [HAVING]
                                // - Not yet

                                // Parse [ORDBER-BY]
                                // - Not yet
                            }
                            break;
                        #endregion

                        #region [TokenType.Insert]
                        case TokenType.Insert:
                            {
                                // Create new select node
                                returnNode = parent.CreateChildNode<InsertNode>(token, symbolTable);

                                SyntaxTreeNode into = null;

                                // Parse [INTO]
                                if (tokens.Count > 0 && tokens.PeekFirst().Type == TokenType.Into)
                                {
                                    into = Parse(returnNode, tokens, ((InsertNode)returnNode).SymbolTable);
                                }
                                else
                                {
                                    ReportError("S1545", "Missing `into` keyword after inser.", token);
                                }

                                // Parse [Column-Definitions-List]
                                if (tokens.Count > 0 && tokens.PeekFirst().Type == TokenType.OpenParenthesis && into != null)
                                {
                                    TokenType[] allowedTypes = { TokenType.Comma, TokenType.Name };
                                    var enclosingTokens = GetInnerTokens(tokens, TokenType.OpenParenthesis, TokenType.CloseParenthesis);

                                    // Remove (
                                    enclosingTokens.PopFirst();

                                    // Remove )
                                    if (enclosingTokens.Count > 0 && enclosingTokens.PeekLast().Type == TokenType.CloseParenthesis)
                                    {
                                        enclosingTokens.PopLast();
                                    }
                                    else
                                    {
                                        errorListener.Report("S5471", "Expected closing parenthesis", token.Index.Item1, token.Index.Item2, token);
                                    }

                                    // Get column list
                                    TokenType lastType = TokenType.Unkown;
                                    foreach (var inner in enclosingTokens)
                                    {
                                        if (inner.Type == TokenType.NewLine)
                                        {
                                            continue;
                                        }

                                        if (allowedTypes.Contains(inner.Type))
                                        {
                                            if (lastType == TokenType.Unkown || lastType == TokenType.Comma)
                                            {
                                                if (inner.Type == TokenType.Name)
                                                {
                                                    // Add column
                                                    var column = into.CreateChildNode<ColumnNode>(inner);
                                                }
                                                else
                                                {
                                                    errorListener.Report("S1288", "Exptected column name", token.Index.Item1, token.Index.Item2, token);
                                                }
                                            }
                                            if (lastType == TokenType.Name)
                                            {
                                                if (inner.Type != TokenType.Comma)
                                                {
                                                    errorListener.Report("S1288", "Unexpected token. Expected `,`", token.Index.Item1, token.Index.Item2, token);
                                                }
                                            }
                                            lastType = inner.Type;
                                        }
                                        else
                                        {
                                            errorListener.Report("S1287", "Unexptected token in column list", token.Index.Item1, token.Index.Item2, token);
                                        }
                                    }
                                }

                                // Parse [VALUES]
                                if (tokens.Count > 0 && tokens.PeekFirst().Type == TokenType.Values)
                                {
                                    var values = Parse(returnNode, tokens, ((InsertNode)returnNode).SymbolTable);
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Star]
                        case TokenType.Star:
                            {
                                if (parent.NodeType == SyntaxNodeType.ReturnValueList)
                                {
                                    var allColumns = parent.CreateChildNode<AllColumnNode>(token);
                                    exitLoop = true; // We can leave here, because nothing else can be done
                                    returnNode = allColumns; // We must care, that this returnNode is not allowed to get an alias
                                }
                                else
                                {
                                    ReportError("S3185", "Unexpected token `*`", token);
                                }
                                exitLoop = true;
                            }
                            break;
                        #endregion

                        #region [TokenType.From]
                        case TokenType.From:
                            {
                                // If we currently parsing a column list, than a FROm statement should stop here and return.
                                if (parent.NodeType == SyntaxNodeType.ReturnValueList)
                                {
                                    tokens.PushFront(token);
                                    exitLoop = true;
                                    break;
                                }

                                if (parent.NodeType == SyntaxNodeType.Select)
                                {
                                    // This is not a scalar select any more...
                                    ((SelectNode)parent).SelectScalar = false;

                                    var from = parent.CreateChildNode<FromNode>(token);
                                    parent = from;
                                    returnNode = from;
                                }
                                else
                                {
                                    errorListener.Report("S1479", "Unexpected FROM key word.", token.Index.Item1, token.Index.Item2, token);
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Into]
                        case TokenType.Into:
                            {
                                if (parent.NodeType == SyntaxNodeType.Insert)
                                {
                                    returnNode = parent.CreateChildNode<IntoNode>(token);
                                    parent = returnNode;
                                }
                                else
                                {
                                    errorListener.Report("S1479", "Unexpected `into` key word.", token.Index.Item1, token.Index.Item2, token);
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Values]
                        case TokenType.Values:
                            {
                                if (parent.NodeType == SyntaxNodeType.Insert)
                                {
                                    var valuesNode = parent.CreateChildNode<ValuesNode>(token);

                                    if (tokens.Count > 0)
                                    {
                                        var next = tokens.PeekFirst();

                                        // Read columns for: insert into <table-name> ( ... ) VALUES ( <-- this content will be read --> )
                                        if (next.Type == TokenType.OpenParenthesis)
                                        {
                                            TokenType[] allowedTypes = { TokenType.Comma, TokenType.Constant, TokenType.Parameter };
                                            var enclosingTokens = GetInnerTokens(tokens, TokenType.OpenParenthesis, TokenType.CloseParenthesis);

                                            // Remove (
                                            enclosingTokens.PopFirst();

                                            // Remove )
                                            if (enclosingTokens.Count > 0 && enclosingTokens.PeekLast().Type == TokenType.CloseParenthesis)
                                            {
                                                enclosingTokens.PopLast();
                                            }
                                            else
                                            {
                                                errorListener.Report("S5471", "Expected closing parenthesis", token.Index.Item1, token.Index.Item2, token);
                                            }

                                            // Get column list
                                            DummyNode dummy = new DummyNode();
                                            var lst = ParseExpressionList(null, enclosingTokens, symbolTable, false, new TokenType[] { }, dummy);

                                            foreach (var d in dummy.Children)
                                            {
                                                var arg = valuesNode.CreateChildNode<ArgumentNode>(null);
                                                arg.Children.Enqueue(d);
                                                d.ParentNode = arg;
                                            }

                                            dummy.Children.Clear();
                                        }
                                        else if (next.Type == TokenType.Values)
                                        {
                                            // Parse values directly
                                        }
                                        else
                                        {
                                            errorListener.Report("S5497", "Unexptected token after `insert into <table-name>`", token.Index.Item1, token.Index.Item2, token);
                                        }
                                    }
                                    else
                                    {
                                        errorListener.Report("S5488", "Expected column, open parenthesis or values-list after `insert into <table-name>`", token.Index.Item1, token.Index.Item2, token);
                                    }
                                }
                                else
                                {
                                    errorListener.Report("S5815", "Unexpected key word `values` in none insert statement", token.Index.Item1, token.Index.Item2, token);
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Where]
                        case TokenType.Where:
                            {
                                if (parent.NodeType == SyntaxNodeType.Select)
                                {
                                    var where = parent.CreateChildNode<WhereNode>(token);

                                    Parse(where, tokens, symbolTable);

                                    returnNode = where;
                                    exitLoop = true;
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Owner]
                        case TokenType.Owner:
                            {
                                string owner = GetOwnerAsString(token, tokens);

                                if (parent.NodeType == SyntaxNodeType.From || parent.NodeType == SyntaxNodeType.Into)
                                {
                                    if (tokens.Count == 0 || (tokens.PeekFirst().Type != TokenType.Name && tokens.PeekFirst().Type != TokenType.Star))
                                    {
                                        ReportError("S2182", "Table name.", token);
                                    }
                                    else
                                    {
                                        if (tokens.PeekFirst().Type == TokenType.Name)
                                        {
                                            var tableNode = parent.CreateChildNode<TableNode>(tokens.PopFirst());
                                            tableNode.Owner = owner;

                                            if (parent.NodeType == SyntaxNodeType.From)
                                            {
                                                TryGenerateAlias(tableNode, tokens);
                                            }
                                        }
                                    }

                                    // We don't want to do anything else here. because a From/Into statement stays on his own.
                                    exitLoop = true;
                                }
                                else
                                {
                                    if (tokens.Count == 0 || (tokens.PeekFirst().Type != TokenType.Name && tokens.PeekFirst().Type != TokenType.Star))
                                    {
                                        ReportError("S2182", "Expected column/* after owner.", token);
                                    }
                                    else
                                    {
                                        if (tokens.PeekFirst().Type == TokenType.Name)
                                        {
                                            var columnNode = new ColumnNode(parent, tokens.PopFirst());
                                            treeNodeList.Add(columnNode);
                                            columnNode.Owner = owner;
                                        }
                                        else
                                        {
                                            var allColumns = parent.CreateChildNode<AllColumnNode>(tokens.PopFirst());
                                            allColumns.Owner = owner;
                                            exitLoop = true; // We can leave here, because nothing else can be done
                                            returnNode = allColumns; // We must care, that this returnNode is not allowed to get an alias
                                        }
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Name]
                        // Names can be columns, functions or table names.
                        case TokenType.Name:
                            {
                                RawToken nextToken = null;

                                // Get the next possible token
                                if (tokens.Count > 0)
                                {
                                    nextToken = tokens.PeekFirst();
                                }

                                // It must be a function (for example select len('123')
                                if (nextToken != null && nextToken.Type == TokenType.OpenParenthesis && parent.NodeType != SyntaxNodeType.Into)
                                {
                                    var functionCall = new CallFunctionNode(null, token);
                                    treeNodeList.Add(functionCall);
                                    if (tokens.Count > 0)
                                    {
                                        var enclosingTokens = GetInnerTokens(tokens, TokenType.OpenParenthesis, TokenType.CloseParenthesis);

                                        // Remove (
                                        enclosingTokens.PopFirst();

                                        // Remove )
                                        if (enclosingTokens.Count > 0 && enclosingTokens.PeekLast().Type == TokenType.CloseParenthesis)
                                        {
                                            enclosingTokens.PopLast();
                                        }
                                        else
                                        {
                                            errorListener.Report("S5471", "Expected closing parenthesis", token.Index.Item1, token.Index.Item2, token);
                                        }

                                        // Get column list and attach to argument list
                                        var returnList = ParseExpressionList(null, enclosingTokens, symbolTable, false, new TokenType[] { });
                                        
                                        foreach (var tmp in returnList.Children)
                                        {
                                            var arg = functionCall.CreateChildNode<ArgumentNode>(null);
                                            arg.Children.Enqueue(tmp);
                                        }
                                        returnList.Children.Clear();
                                        // ---

                                        TryGenerateAlias(functionCall, tokens);
                                    }
                                    else
                                    {
                                        errorListener.Report("S5412", "Expected open parenthesis function call", token.Index.Item1, token.Index.Item2, token);
                                    }
                                }
                                else if (parent.NodeType == SyntaxNodeType.From || parent.NodeType == SyntaxNodeType.Into)
                                {
                                    var tableNode = parent.CreateChildNode<TableNode>(token);

                                    if (parent.NodeType == SyntaxNodeType.From)
                                    {
                                        TryGenerateAlias(tableNode, tokens);
                                    }

                                    // exit here
                                    exitLoop = true;
                                }
                                // It must be a colum which should be selected. (for example select a from b)
                                else
                                {
                                    // Check whether it might be a column or just an alias
                                    if (lastToken == null
                                        || lastToken.Type == TokenType.And
                                        || lastToken.Type == TokenType.Or
                                        || lastToken.Type == TokenType.EqualOrSet
                                        || lastToken.Type == TokenType.Greater
                                        || lastToken.Type == TokenType.Smaller
                                        || lastToken.Type == TokenType.GreaterEqual
                                        || lastToken.Type == TokenType.SmallerEqual
                                        || lastToken.Type == TokenType.Add
                                        || lastToken.Type == TokenType.Subtract
                                        || lastToken.Type == TokenType.Devide
                                        || lastToken.Type == TokenType.Multiply)
                                    {
                                        var columnNode = new ColumnNode(parent, token);
                                        treeNodeList.Add(columnNode);
                                    }
                                    else
                                    {
                                        // Push alias back
                                        tokens.PushFront(token);
                                        exitLoop = true; // finish if is alias
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.Constant]
                        case TokenType.Constant:
                            {
                                var constNode = new ConstantNode(null, token);

                                var constType = constNode.DataType = parserConfig.GetConstDataType(token);

                                if (constType == DataType.None)
                                {
                                    constNode.DataType = DataType.None;
                                    errorListener.Report("ST0010", "Could not detect format for constant: " + token.Content, token.Index.Item1, token.Index.Item2, token);
                                }

                                treeNodeList.Add(constNode);
                            }
                            break;
                        #endregion

                        #region [TokenType.Operator]
                        case TokenType.Add:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Add;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Subtract:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Sub;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Multiply:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Mul;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Devide:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Div;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;
                        case TokenType.EqualOrSet:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Equal;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Unequal:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Unequal;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Greater:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Greater;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Smaller:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Smaller;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.GreaterEqual:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.GreaterEqual;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.SmallerEqual:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.SmallerEqual;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;
                        case TokenType.And:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.And;
                                opNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Or:
                            {
                                OperatorNode opNode = new OperatorNode(null, token);
                                opNode.OperatorType = OperatorType.Or;
                                opNode.Association = OperatorAssociation.Left;
                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.Not:
                            {
                                treeNodeList.Add(new ConstantNode(null, new RawToken("false", null, null)) { DataType = DataType.Boolean });
                                treeNodeList.Add(new OperatorNode(null, new RawToken("=", null, null)) { OperatorType = OperatorType.Equal, Association = OperatorAssociation.Left });
                            }
                            break;
                        #endregion

                        #region [TokenType.Parenthesis]
                        case TokenType.OpenParenthesis:
                            {
                                ParenthesisNode opNode = new ParenthesisNode(null, token);
                                opNode.Type = BracketType.Open;

                                treeNodeList.Add(opNode);
                            }
                            break;

                        case TokenType.CloseParenthesis:
                            {
                                ParenthesisNode opNode = new ParenthesisNode(null, token);
                                opNode.Type = BracketType.Close;

                                treeNodeList.Add(opNode);
                            }
                            break;
                        #endregion

                        #region [TokenType.Comma]
                        case TokenType.Comma:
                            // Cancel the "loop"
                            if (parent.NodeType == SyntaxNodeType.ReturnValueList 
                                || parent.NodeType == SyntaxNodeType.Argument 
                                || parent.NodeType == SyntaxNodeType.Dummy)
                            {
                                tokens.PushFront(token);
                            }
                            else
                            {
                                ReportError("S1568", "Unexpected token `,`", token);
                            }

                            exitLoop = true;
                            break;
                        #endregion

                        #region [TokenType.Distinct]
                        case TokenType.Distinct:
                            if (parent.NodeType != SyntaxNodeType.ReturnValueList || parent.Children.Count > 0 || parent.ParentNode.NodeType != SyntaxNodeType.Select)
                            {
                                errorListener.Report("S4721", "Distinct is only allowed after select key-word.", token.Index.Item1, token.Index.Item2, token);
                            }
                            else
                            {
                                // Attach to select
                                parent.ParentNode.CreateChildNode<DistinctNode>(token);
                                exitLoop = true;
                            }
                            break;
                        #endregion

                        #region [TokenType.First]
                        case TokenType.First:
                            if (parent.NodeType != SyntaxNodeType.ReturnValueList || parent.Children.Count > 0 || parent.ParentNode.NodeType != SyntaxNodeType.Select)
                            {
                                errorListener.Report("S4722", "First is only allowed after select key-word.", token.Index.Item1, token.Index.Item2, token);
                            }
                            else
                            {
                                // Attach to select
                                parent.ParentNode.CreateChildNode<FirstNode>(token);
                                exitLoop = true;
                            }
                            break;
                        #endregion

                        #region [TokenType.In]
                        case TokenType.In:
                            {
                                var inNode = new OperatorNode(null, token);
                                inNode.OperatorType = OperatorType.Equal;
                                inNode.Association = OperatorAssociation.Left;

                                treeNodeList.Add(inNode);

                                if (tokens.Count > 0)
                                {
                                    var next = tokens.PeekFirst();

                                    // Read columns for: in (...)
                                    if (next.Type == TokenType.OpenParenthesis)
                                    {
                                        var enclosingTokens = GetInnerTokens(tokens, TokenType.OpenParenthesis, TokenType.CloseParenthesis);

                                        // Remove (
                                        enclosingTokens.PopFirst();

                                        // Remove )
                                        if (enclosingTokens.Count > 0 && enclosingTokens.PeekLast().Type == TokenType.CloseParenthesis)
                                        {
                                            enclosingTokens.PopLast();
                                        }
                                        else
                                        {
                                            errorListener.Report("S5471", "Expected closing parenthesis", token.Index.Item1, token.Index.Item2, token);
                                        }

                                        // Get column list
                                        var lst = ParseExpressionList(null, enclosingTokens, symbolTable, false, new TokenType[] { });
                                        treeNodeList.Add(lst);
                                    }
                                    else
                                    {
                                        errorListener.Report("S5441", "Unexptected token after `in` keyword", token.Index.Item1, token.Index.Item2, token);
                                    }
                                }
                                else
                                {
                                    errorListener.Report("S5412", "Expected column, open parenthesis or values-list after `in` keyword", token.Index.Item1, token.Index.Item2, token);
                                }
                            }
                            break;
                        #endregion

                        #region [TokenType.As]
                        case TokenType.As:
                            // Cancel the "loop" and push back As keyword, because it should be parsed using TryGenerateAlias
                            exitLoop = true;
                            if (parent.ParentNode.NodeType == SyntaxNodeType.ReturnValueList && parent.NodeType != SyntaxNodeType.AllColumn)
                            {
                                tokens.PushFront(token);
                            }
                            break;
                        #endregion

                        #region [Default]
                        default:
                            errorListener.Report("ST0006", "Token type not handled: " + token.Content, token.Index.Item1, token.Index.Item2, token);
                            break;
                        #endregion
                    }

                    lastToken = token;

                    if (exitLoop)
                    {
                        break;
                    }
                }

                #region [Shunting-Yard]
                // Check wether there are tokens which should be ordered with the shunting-yard
                // For example (1 == 2 / 3)...
                if (treeNodeList.Count > 0)
                {
                    // Build the SyntaxTree nodes from the tokens-list
                    int openTerms = tokens.OfType<ParenthesisNode>().Where(Item => Item.Type == BracketType.Open).ToList().Count;
                    int closeTerms = tokens.OfType<ParenthesisNode>().Where(Item => Item.Type == BracketType.Close).ToList().Count;

                    if (openTerms > closeTerms)
                    {
                        errorListener.Report("ST0007", "Syntax-Error: near line: . Missing ): " + treeNodeList.First().Token.Content, treeNodeList.First().Token.Index.Item1, treeNodeList.First().Token.Index.Item2, treeNodeList.First().Token);
                    }
                    else if (openTerms < closeTerms)
                    {
                        errorListener.Report("ST0008", "Syntax-Error: near line: . Missing (: " + treeNodeList.First().Token.Content, treeNodeList.First().Token.Index.Item1, treeNodeList.First().Token.Index.Item2, treeNodeList.First().Token);
                    }

                    // Reorder tokens with the shunting-yard algorithm
                    ShuntingYard yard = new ShuntingYard(treeNodeList);
                    List<SyntaxTreeNode> postFixTokenized = yard.Execute().ToList();

                    // Parse Post-Fix to Binary tree of Tokenized-Elements
                    Stack<SyntaxTreeNode> tempStack = new Stack<SyntaxTreeNode>();
                    SyntaxTreeNode lastNode = null;

                    foreach (SyntaxTreeNode orderedToken in postFixTokenized)
                    {
                        if (!ShuntingYard.TokenIsOperator(orderedToken))
                        {
                            tempStack.Push(orderedToken);
                        }
                        else
                        {
                            var firstNode = tempStack.Pop();
                            orderedToken.Children.Enqueue(firstNode);
                            firstNode.ParentNode = orderedToken;

                            var secondNode = tempStack.Pop();
                            orderedToken.Children.Enqueue(secondNode);
                            secondNode.ParentNode = orderedToken;

                            tempStack.Push(orderedToken);
                        }

                        lastNode = orderedToken;
                    }

                    parent.Children.Enqueue(lastNode);
                    lastNode.ParentNode = parent;
                    returnNode = lastNode ?? parent;
                }
                #endregion
            }
            catch
            {
                if (returnNode != null && returnNode.Token != null)
                {
                    ReportError("SY01", String.Format("Unexpected token near: `{0}`", returnNode.Token.Content ?? ""), returnNode.Token);
                }
                if (parent != null && parent.Token != null)
                {
                    ReportError("SY02", String.Format("Unexpected token near: `{0}`", parent.Token.Content ?? ""), parent.Token);
                }
                if (lastToken != null)
                {
                    ReportError("SY03", String.Format("Unexpected token near: `{0}`", lastToken.Content ?? ""), lastToken);
                }
                else
                {
                    ReportError("SY00", "Unexpected token in line 1", null);
                }
            }

            return returnNode;
        }
        #endregion

        #region [GetOwnerAsString]
        /// <summary>
        /// Get owner as string `dbo`.`user`.`table`, ...
        /// </summary>
        /// <param name="startOwner">Owner which will be appended first</param>
        /// <param name="tokens">List of tokens for owner detection</param>
        /// <returns>Owner as string</returns>
        private string GetOwnerAsString(RawToken startOwner, Dequeue<RawToken> tokens)
        {
            StringBuilder owner = new StringBuilder();
            owner.Append(startOwner.Content);

            while (tokens.Count > 0)
            {
                if (tokens.PeekFirst().Type == TokenType.Owner)
                {
                    if (owner.Length > 0)
                    {
                        owner.Append(".");
                    }

                    owner.Append(tokens.PopFirst().Content);
                }
                else
                {
                    break;
                }
            }

            if (owner.Length == 0)
            {
                return null;
            }

            return owner.ToString();
        }
        #endregion

        #region [ParseExpressionList]
        /// <summary>
        /// Parse an expression list. For example select 1, 2, 3 or for insert into statements.
        /// </summary>
        /// <param name="parent">Parent to attach to</param>
        /// <param name="tokens">Available tokens</param>
        /// <param name="symbolTable">Current symbol table</param>
        /// <param name="allowAlias">If alias can be attached to an item in the expression list</param>
        /// <param name="cancelTokens">Tokens which cancel expression list parsing</param>
        /// <param param name="overridingList">Will be used and returned instead of ReturnValueList. For example used for return ArgumentList.</param>
        private SyntaxTreeNode ParseExpressionList(SyntaxTreeNode parent, Dequeue<RawToken> tokens, SymbolTable symbolTable, bool allowAlias, TokenType[] cancelTokens, SyntaxTreeNode overridingList = null)
        {
            SyntaxTreeNode returnValue = overridingList;

            if (parent == null && returnValue == null)
            {
                returnValue = new ReturnValueList(null, null);
            }
            else if (returnValue == null)
            {
                returnValue = parent.CreateChildNode<ReturnValueList>(null);
            }

            int i = 0;

            while (tokens.Count > 0)
            {
                var token = tokens.PeekFirst();

                // Exit on cancel token
                if (cancelTokens.Contains(token.Type))
                {
                    break;
                }
                else if (i % 2 == 1 && token.Type == TokenType.Comma)
                {
                    // jump over comma
                    tokens.PopFirst();
                }
                // If , was expeceted but is no comma
                else if (i % 2 == 1 && token.Type != TokenType.Comma)
                {
                    ReportError("S1575", "Expected `,`.", token);
                }
                else if (i % 2 == 0 && token.Type == TokenType.Comma)
                {
                    ReportError("S1576", "Unexpected token `,`", token);
                }
                else
                {
                    var returnNode = Parse(returnValue, tokens, symbolTable);
                    if (allowAlias)
                    {
                        TryGenerateAlias(returnNode, tokens);
                    }
                }

                i++;
            }

            return returnValue;
        }
        #endregion

        #region [TryGenerateAlias]
        /// <summary>
        /// Tries to add an alias to a table, view or selected / computed column
        /// </summary>
        /// <param name="parent">Parent to attach to (table, ...)</param>
        /// <param name="tokens">Available tokens</param>
        private void TryGenerateAlias(SyntaxTreeNode parent, Dequeue<RawToken> tokens)
        {
            if (tokens.Count > 0)
            {
                var aliasToken = tokens.PeekFirst();

                // check the following cases: select 1 a or select 1 as a
                if (aliasToken.Type == TokenType.As || aliasToken.Type == TokenType.Name)
                {
                    var asNode = parent.CreateChildNode<AsNode>(aliasToken);

                    // select 1 as a
                    if (aliasToken.Type == TokenType.As)
                    {
                        tokens.PopFirst(); // remove as
                        if (tokens.Count > 0)
                        {
                            var aliasName = tokens.PeekFirst();
                            if (aliasName.Type == TokenType.Name)
                            {
                                tokens.PopFirst(); // remove name behind as
                                asNode.Alias = aliasName.Content;
                            }
                            else
                            {
                                ReportError("S5318", "Expected alias name after `as`-key word.", aliasToken);
                            }
                        }
                        else
                        {
                            ReportError("S5318", "Expected alias name after `as`-key word.", aliasToken);
                        }
                    }
                    // select 1 a
                    else if (aliasToken.Type == TokenType.Name)
                    {
                        tokens.PopFirst(); // remove name token
                        asNode.Alias = aliasToken.Content;
                    }
                }
            }
        }
        #endregion

        private void ReportError(string errorCode, string message, RawToken token)
        {
            errorListener.Report(errorCode, message, token.Index.Item1, token.Index.Item2, token);
        }

        #region Private Methods
        /// <summary>
        /// Get every token between two tokens
        /// </summary>
        /// <param name="tokens">Token-List</param>
        /// <param name="openToken"></param>
        /// <param name="closeToken"></param>
        /// <returns>List of new tokens</returns>
        private Dequeue<RawToken> GetInnerTokens(Dequeue<RawToken> tokens, TokenType openToken, TokenType closeToken)
        {
            Dequeue<RawToken> returnList = new Dequeue<RawToken>();

            int bracketCounting = 0;
            while (tokens.Count > 0)
            {
                RawToken token = tokens.PopFirst();

                if (token.Type == openToken)
                {
                    bracketCounting++;
                }
                else if (token.Type == closeToken)
                {
                    bracketCounting--;
                }

                returnList.PushBack(token);

                // Remove added token
                if (bracketCounting == 0)
                {
                    break;
                }
            }

            return returnList;
        }

        /// <summary>
        /// Get all token until a specific token appears
        /// </summary>
        /// <param name="tokens">List of tokens</param>
        /// <param name="untilToken">Stop token</param>
        /// <returns>New list of tokens</returns>
        private Dequeue<RawToken> GetUntilTokens(Dequeue<RawToken> tokens, TokenType untilToken)
        {
            Dequeue<RawToken> returnList = new Dequeue<RawToken>();

            while (tokens.Count > 0)
            {
                RawToken token = tokens.PopFirst();

                returnList.PushBack(token);

                if (token.Type == untilToken)
                {
                    break;
                }
            }

            return returnList;
        }

        /// <summary>
        /// Get a list with all argumetns
        /// </summary>
        /// <param name="tokens">Token List</param>
        /// <returns></returns>
        private IList<Dequeue<RawToken>> GetArguments(Dequeue<RawToken> tokens)
        {
            IList<Dequeue<RawToken>> returnValue = new List<Dequeue<RawToken>>();
            int bracketCounter = 0;
            Dequeue<RawToken> currentList = new Dequeue<RawToken>();

            while (tokens.Count > 0)
            {
                RawToken currentToken = tokens.PopFirst();

                if (currentToken.Type == TokenType.OpenParenthesis || currentToken.Type == TokenType.OpenBracket || currentToken.Type == TokenType.OpenBraceToken)
                {
                    bracketCounter++;
                    currentList.PushBack(currentToken);
                }
                else if (currentToken.Type == TokenType.CloseParenthesis || currentToken.Type == TokenType.CloseBracket || currentToken.Type == TokenType.CloseBraceToken)
                {
                    bracketCounter--;
                    currentList.PushBack(currentToken);
                }
                else if (currentToken.Type == TokenType.Comma && bracketCounter == 0)
                {
                    if (currentList.Count == 0)
                    {
                        errorListener.Report("ST0009", "Token type not handled: " + currentToken.Content, currentToken.Index.Item1, currentToken.Index.Item2, currentToken);
                    }

                    returnValue.Add(currentList);
                    currentList = new Dequeue<RawToken>();
                }
                else
                {
                    currentList.PushBack(currentToken);
                }
            }

            if (currentList.Count > 0)
            {
                returnValue.Add(currentList);
            }

            return returnValue;
        }
        #endregion
    }
}
