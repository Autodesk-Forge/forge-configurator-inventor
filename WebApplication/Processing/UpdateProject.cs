/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

namespace webapplication.Processing
{
    /// <summary>
    /// Update project:
    /// - generate SVF
    /// - extract parameters and BOM
    /// - save current model
    /// </summary>
    public class UpdateProject : AggregatedDefinition
    {
        public UpdateProject(Publisher publisher) :
            base(publisher,
                new UpdateParameters(publisher),
                new CreateSVF(publisher),
                new CreateBOM(publisher))
        {
        }

        public override string Id => nameof(UpdateProject);
        public override string Description => "Update parameters";
    }
}
