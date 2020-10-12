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

// compute text width from canvas
// don't want to test string width calc:
/* istanbul ignore next */
export function getMaxColumnTextWidth(strings) {
    const font = "13px ArtifaktElement, sans-serif";
    const canvas = document.createElement("canvas");
    const context2d = canvas.getContext("2d");
    context2d.font = font;
    let maxWidth = 0;
    strings.forEach(element => {
      const width = context2d.measureText(element).width;
      maxWidth = width>maxWidth ? width : maxWidth;
    });

    // round to 10 times number, like 81.5 -> 90, 87.1 -> 90, etc
    const roundTo = 10;
    const rounded = (maxWidth % roundTo==0) ? maxWidth : maxWidth-maxWidth%roundTo + roundTo;
    return rounded;
  }