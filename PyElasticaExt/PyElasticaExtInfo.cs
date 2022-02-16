using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace PyElasticaExt
{
    public class PyElasticaExtInfo : GH_AssemblyInfo
    {
        public override string Name => "PyElasticaExtension";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("C49BCF94-08E8-43E3-8C2B-05CB069E770D");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}