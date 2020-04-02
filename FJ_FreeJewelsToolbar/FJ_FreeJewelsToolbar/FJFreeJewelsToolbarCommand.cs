using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace FJ_FreeJewelsToolbar
{
    public class FJFreeJewelsToolbarCommand : Command
    {
        public FJFreeJewelsToolbarCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FJFreeJewelsToolbarCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FJ_FreeJewelsToolbar"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            
            RhinoApp.WriteLine("The {0} command does nothing but being a friend to the toolbar :).", EnglishName);
            
            return Result.Success;
        }
    }
}
