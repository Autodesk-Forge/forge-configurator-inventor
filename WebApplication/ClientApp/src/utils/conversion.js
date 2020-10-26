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

// returns string representing passed in number with at least minimumDigits significant digits,
// without removing any significant digits before decimal point and
// without adding any zeros after decimal point
export const significantDigits = (number, minimumDigits) => {
    if(number == null) {
        return '';
    }
    if(isNaN(number)) {
        return number;
    }
    const digitsBeforeDecimal = Math.floor(Math.log10(number)+1.0);
    const trueSignificantDigits = digitsBeforeDecimal > minimumDigits ? digitsBeforeDecimal : minimumDigits;
    // is the number, as is, shorter than required digits? don't add anything
    const strLen = number.toString().length;
    const hasDecimal = strLen > digitsBeforeDecimal;
    if(hasDecimal && strLen-1 < trueSignificantDigits /* for the decimal point */) {
        return number.toString();
    }

    return number.toPrecision(trueSignificantDigits);
};

// converts an array of (adoption) warnings into the final, single warning string
export function fullWarningMsg( warningsArray, separator = '\n', extraMessages = [ 'Change of parameters may lead to incorrect results.' ] ) {
    if (!(warningsArray?.length > 0))
        return null;

    const newArray = [ ...warningsArray, ...extraMessages ];
    return newArray.join(separator);
};