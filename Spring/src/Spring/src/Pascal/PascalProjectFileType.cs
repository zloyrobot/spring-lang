using System.Collections.Generic;
using JetBrains.ProjectModel;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    [ProjectFileTypeDefinition(Name)]
    public class PascalProjectFileType : KnownProjectFileType
    {
        private new const string Name = "Pascal";

        private PascalProjectFileType() : base(Name, "Pascal", new[] {PascalExtension})
        {
        }

        protected PascalProjectFileType(string name) : base(name)
        {
        }

        protected PascalProjectFileType(string name, string presentableName) : base(name, presentableName)
        {
        }

        protected PascalProjectFileType(string name, string presentableName, IEnumerable<string> extensions) : base(name,
            presentableName, extensions)
        {
        }

        private const string PascalExtension = ".pas";
    }
}
