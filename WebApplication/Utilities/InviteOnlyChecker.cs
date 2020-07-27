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

using System;
using System.Linq;
using System.Net.Mail;
using WebApplication.Definitions;

public class InviteOnlyChecker
{
    private readonly InviteOnlyModeConfiguration _inviteOnlyModeConfig;

    public InviteOnlyChecker(InviteOnlyModeConfiguration inviteOnlyModeConfig)
    {
        _inviteOnlyModeConfig = inviteOnlyModeConfig;
    }

    public bool IsInvited(string email)
    {
        if (_inviteOnlyModeConfig.Enabled)
        {
            bool isInDomains = false;
            if (_inviteOnlyModeConfig.Domains?.Length > 0)
            {
                MailAddress address = new MailAddress(email); // email comes from autodesk auth service so it should be in a valid format
                string host = address.Host;
                isInDomains = _inviteOnlyModeConfig.Domains.Contains(host);
            }

            bool isInAddresses = false;
            if (_inviteOnlyModeConfig.Addresses?.Length > 0)
            {
                isInAddresses = _inviteOnlyModeConfig.Addresses.Contains(email);
            }

            return isInDomains || isInAddresses;
        }

        return true;
    }
}