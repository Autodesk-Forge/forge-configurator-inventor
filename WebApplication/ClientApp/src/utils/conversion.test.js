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

import { significantDigits } from './conversion';

describe('Conversion utils', () => {
    describe('Significant digits conversion', () => {
        it('Basic conversion', async () => {
            expect(significantDigits(1.2345, 3)).toEqual('1.23');
        });
        it('Small number conversion with round', async () => {
            expect(significantDigits(0.023456, 3)).toEqual('0.0235');
        });
        it('Not adding zeros after decimal point', async () => {
            expect(significantDigits(1.2, 3)).toEqual('1.2'); // and not '1.20'
        });
        it('Not removing significant digits before decimal point', async () => {
            expect(significantDigits(1234.56, 2)).toEqual('1235'); // and not '1200'
        });
        it('Does not alter string', () => {
            expect(significantDigits('s123.456', 2)).toEqual('s123.456');
        });
        it('Does not fail on undefined nor null', () => {
            expect(significantDigits(undefined, 2)).toEqual('');
            expect(significantDigits(null, 2)).toEqual('');
        });
    });
});