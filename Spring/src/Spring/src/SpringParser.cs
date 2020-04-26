using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Host.Features.ProjectModel.Diagnostic.FrameworkAnalyzers;
using JetBrains.ReSharper.I18n.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.TreeBuilder;
using JetBrains.Text;
using Mono.CSharp;

namespace JetBrains.ReSharper.Plugins.Spring
{
    internal class SpringParser : IParser
    {
        private readonly ILexer myLexer;

        public SpringParser(ILexer lexer)
        {
            myLexer = lexer;
        }

        public IFile ParseFile()
        {
            using (var def = Lifetime.Define())
            {
                var builder = new PsiBuilder(myLexer, SpringFileNodeType.Instance, new TokenFactory(), def.Lifetime);
                var fileMark = builder.Mark();
                ParseSeq(builder);
                builder.Done(fileMark, SpringFileNodeType.Instance, null);
                var file = (IFile)builder.BuildTree();
                var stringBuilder = new StringBuilder();
                DebugUtil.DumpPsi(new StringWriter(stringBuilder), file);
                stringBuilder.ToString();
                return file;
            }
        }
        
        private bool ParseSeq(PsiBuilder builder)
        {

            var start = builder.Mark();

            if (!ParseStmt(builder))
            {
                builder.Drop(start);
                return false;
            }
            
            AdvanceWithSpaces(builder);
            
            if (builder.Eof() || builder.GetTokenType() == SpringTokenType.RFBRACKET)
            {
                builder.Drop(start);
                return true;
            }

            if (!ParseSeq(builder))
            {
                builder.Drop(start);
                return false;
            }
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.SEQ, null);
            return true;
        }

        private bool ParseStmt(PsiBuilder builder)
        {
            var start = builder.Mark();

            if (builder.GetTokenType() == SpringTokenType.FOR)
            {
                AdvanceWithSpaces(builder);
                if (builder.GetTokenType() != SpringTokenType.LBRACKET)
                {
                    builder.Drop(start);
                    builder.Error("Missing '('");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (!ParseAssign(builder))
                {
                    builder.Drop(start);
                    return false;
                }
                if (builder.GetTokenType() != SpringTokenType.SEQ)
                {
                    builder.Drop(start);
                    builder.Error("Missing ';'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (!ParseLogic(builder)) 
                {
                    builder.Drop(start);
                    return false;
                }
                if (builder.GetTokenType() != SpringTokenType.SEQ)
                {
                    builder.Drop(start);
                    builder.Error("Missing ';'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (!ParseAssign(builder))
                {
                    builder.Drop(start);
                    return false;
                }
                if (builder.GetTokenType() != SpringTokenType.RBRACKET)
                {
                    builder.Drop(start);
                    builder.Error("Missing ')'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (builder.GetTokenType() != SpringTokenType.LFBRACKET)
                {
                    builder.Drop(start);
                    builder.Error("Missing '{'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (!ParseSeq(builder))
                {
                    builder.Drop(start);
                    return false;
                }
                if (builder.GetTokenType() != SpringTokenType.RFBRACKET)
                {
                    builder.Drop(start);
                    builder.Error("Missing '}'");
                    return false;
                }
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.FOR, null);
                return true;
            }

            if (builder.GetTokenType() == SpringTokenType.WRITE)
            {
                AdvanceWithSpaces(builder);
                if (builder.GetTokenType() != SpringTokenType.LBRACKET)
                {
                    builder.RollbackTo(start);
                    builder.Error("Missing '('");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (!ParseLogic(builder))
                {
                    builder.Drop(start);
                    return false;
                }
                if (builder.GetTokenType() != SpringTokenType.RBRACKET)
                {
                    builder.RollbackTo(start);
                    builder.Error("Missing ')'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.WRITE, null);
                
                if (builder.GetTokenType() != SpringTokenType.SEQ)
                {
                    builder.Error("Missing ';'");
                    return false;
                }
                
                return true;
            }

            if (!ParseAssign(builder))
            {
                builder.Drop(start);
                return false;
            }
            
            if (builder.GetTokenType() != SpringTokenType.SEQ)
            {
                builder.Drop(start);
                builder.Error("Missing ';'");
                return false;
            }

            builder.Drop(start);
            return true;
        }

        private bool ParseAssign(PsiBuilder builder)
        {
            var start = builder.Mark();
            
            if (builder.GetTokenType() != SpringTokenType.IDENT)
            {
                builder.Drop(start);
                builder.Error("Missing variable");
                return false;
            }

            AdvanceWithSpaces(builder);
            
            if (builder.GetTokenType() != SpringTokenType.ASSIGN)
            {
                builder.Drop(start);
                builder.Error("Missing ':='");
                return false;
            }
            
            AdvanceWithSpaces(builder);

            if (!ParseLogic(builder))
            {
                builder.Drop(start);
                return false;
            }

            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.ASSIGN, null);
            return true;
        }

        private bool ParseLogic(PsiBuilder builder)
        {
            var start = builder.Mark();

            if (!ParseLow(builder))
            {
                builder.Drop(start);
                return false;
            }
            
            while (builder.GetTokenType() == SpringTokenType.LOGIC_BINOP)
            {
                AdvanceWithSpaces(builder);
                if (!ParseLow(builder))
                {
                    builder.Drop(start);
                    return false;
                }

                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.LOGIC_BINOP, null);
                builder.Precede(start);
            }

            builder.Drop(start);
            return true;
        }
        private bool ParseLow(PsiBuilder builder)
        {
            var start = builder.Mark();

            if (!ParseMedium(builder))
            {
                builder.Drop(start);
                return false;
            }
            
            while (builder.GetTokenType() == SpringTokenType.LOW_BINOP)
            {
                AdvanceWithSpaces(builder);
                if (!ParseMedium(builder))
                {
                    builder.Drop(start);
                    return false;
                }

                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.LOW_BINOP, null);
                builder.Precede(start);
            }

            builder.Drop(start);
            return true;
        }
        
        private bool ParseMedium(PsiBuilder builder)
        {
            var start = builder.Mark();

            if (!ParseHigh(builder))
            {
                builder.Drop(start);
                return false;
            }

            while (builder.GetTokenType() == SpringTokenType.MEDIUM_BINOP)
            {
                AdvanceWithSpaces(builder);
                if (!ParseHigh(builder))
                {
                    builder.Drop(start);
                    return false;
                }
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.MEDIUM_BINOP, null);
                builder.Precede(start);
            }
            
            builder.Drop(start);
            return true;
        }
        
        private bool ParseHigh(PsiBuilder builder)
        {
            var start = builder.Mark();

            if (!ParseIdent(builder))
            {
                builder.Drop(start);
                return false;
            }

            if (builder.GetTokenType() == SpringTokenType.HIGH_BINOP)
            {
                AdvanceWithSpaces(builder);
                if (!ParseHigh(builder))
                {
                    builder.Drop(start);
                    return false;
                }
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.HIGH_BINOP, null);
                return true;
            }

            builder.Drop(start);
            return true;
        }

        private bool ParseIdent(PsiBuilder builder)
        {
            var start = builder.Mark();

            if (builder.GetTokenType() == SpringTokenType.READ)
            {
                AdvanceWithSpaces(builder);
                if (builder.GetTokenType() != SpringTokenType.LBRACKET)
                {
                    builder.RollbackTo(start);
                    builder.Error("Missing '('");
                    return false;
                }
                AdvanceWithSpaces(builder);
                if (builder.GetTokenType() != SpringTokenType.RBRACKET)
                {
                    builder.RollbackTo(start);
                    builder.Error("Missing ')'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.READ, null);
                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.NUMBER)
            {
                AdvanceWithSpaces(builder);
                builder.Done(start, SpringCompositeNodeType.NUMBER, null);
                return true;
            }

            if (builder.GetTokenType() == SpringTokenType.IDENT)
            {
                AdvanceWithSpaces(builder);
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.VARIABLE, null);
                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.STRING)
            {
                AdvanceWithSpaces(builder);
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.STRING, null);
                return true;
            }

            if (builder.GetTokenType() == SpringTokenType.LBRACKET)
            {
                AdvanceWithSpaces(builder);
                if (!ParseLogic(builder))
                {
                    builder.RollbackTo(start);
                    return false;
                }

                if (builder.GetTokenType() != SpringTokenType.RBRACKET)
                {
                    builder.RollbackTo(start);
                    builder.Error("Missing ')'");
                    return false;
                }
                AdvanceWithSpaces(builder);
                builder.Drop(start);
                return true;
            }
            
            builder.Drop(start);
            builder.Error("Not ident");
            return false;
        }
        private void AdvanceWithSpaces(PsiBuilder builder)
        {
            builder.AdvanceLexer();
            while (builder.GetTokenType() == SpringTokenType.WHITE_SPACE)
                builder.AdvanceLexer();
        }
    }

    [DaemonStage]
    class SpringDaemonStage : DaemonStageBase<SpringFile>
    {
        protected override IDaemonStageProcess CreateDaemonProcess(IDaemonProcess process, DaemonProcessKind processKind, SpringFile file,
            IContextBoundSettingsStore settingsStore)
        {
            return new SpringDaemonProcess(process, file);
        }

        internal class SpringDaemonProcess : IDaemonStageProcess
        {
            private readonly SpringFile myFile;
            public SpringDaemonProcess(IDaemonProcess process, SpringFile file)
            {
                myFile = file;
                DaemonProcess = process;
            }

            public void Execute(Action<DaemonStageResult> committer)
            {
                var highlightings = new List<HighlightingInfo>();
                foreach (var treeNode in myFile.Descendants())
                {
                    if (treeNode is PsiBuilderErrorElement error)
                    {
                        var range = error.GetDocumentRange();
                        highlightings.Add(new HighlightingInfo(range, new CSharpSyntaxError(error.ErrorDescription, range)));
                    }
                }
                
                var result = new DaemonStageResult(highlightings);
                committer(result);
            }

            public IDaemonProcess DaemonProcess { get; }
        }

        protected override IEnumerable<SpringFile> GetPsiFiles(IPsiSourceFile sourceFile)
        {
            yield return (SpringFile)sourceFile.GetDominantPsiFile<SpringLanguage>();
        }
    } 

    internal class TokenFactory : IPsiBuilderTokenFactory
    {
        public LeafElementBase CreateToken(TokenNodeType tokenNodeType, IBuffer buffer, int startOffset, int endOffset)
        {
            return tokenNodeType.Create(buffer, new TreeOffset(startOffset), new TreeOffset(endOffset));
        }
    }

    [ProjectFileType(typeof (SpringProjectFileType))]
    public class SelectEmbracingConstructProvider : ISelectEmbracingConstructProvider
    {
        public bool IsAvailable(IPsiSourceFile sourceFile)
        {
            return sourceFile.LanguageType.Is<SpringProjectFileType>();
        }

        public ISelectedRange GetSelectedRange(IPsiSourceFile sourceFile, DocumentRange documentRange)
        {
            var file = (SpringFile) sourceFile.GetDominantPsiFile<SpringLanguage>();
            var node = file.FindNodeAt(documentRange);
            return new SpringTreeNodeSelection(file, node);
        }

        public class SpringTreeNodeSelection : TreeNodeSelection<SpringFile>
        {
            public SpringTreeNodeSelection(SpringFile fileNode, ITreeNode node) : base(fileNode, node)
            {
            }

            public override ISelectedRange Parent => new SpringTreeNodeSelection(FileNode, TreeNode.Parent);
        }
    }
}
