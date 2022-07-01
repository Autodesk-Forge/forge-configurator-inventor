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

using System.Collections.Generic;
using WebApplication.Definitions;

namespace WebApplication.State
{
    public class Uploads
    {
        private readonly Dictionary<string, string> _uploadFilenames;
        private readonly Dictionary<string, ProjectInfo> _uploadProjects;

        public Uploads()
        {
            _uploadFilenames = new Dictionary<string, string>();
            _uploadProjects = new Dictionary<string, ProjectInfo>();
        }

        public void AddUploadData(string uploadId, ProjectInfo projectInfo, string filename)
        {
            _uploadFilenames.Add(uploadId, filename);
            _uploadProjects.Add(uploadId, projectInfo);
        }

        public (ProjectInfo projectInfo, string filename) GetUploadData(string uploadId)
        {
            return (projectInfo: _uploadProjects[uploadId], filename: _uploadFilenames[uploadId]);
        }

        public void ClearUploadData(string uploadId)
        {
            _uploadFilenames.Remove(uploadId);
            _uploadProjects.Remove(uploadId);
        }
    }
}
