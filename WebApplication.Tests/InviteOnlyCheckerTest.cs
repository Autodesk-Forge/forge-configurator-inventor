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

using WebApplication.Definitions;
using Xunit;

namespace WebApplication.Tests
{
    public class InviteOnlyCheckerTest
    {
        [Fact]
        public void IsInvitedDisabledTest()
        {
            string testEmail = "test@anything.com";

            var inviteOnlyConfig = new InviteOnlyModeConfiguration { Enabled = false, Domains = new string[] { }, Addresses = new string[] { } };
            InviteOnlyChecker checker = new InviteOnlyChecker(inviteOnlyConfig);

            bool isInvited = checker.IsInvited(testEmail);
            Assert.True(isInvited);
        }

        [Fact]
        public void IsInvitedDomainTest()
        {
            string invitedDomainEmail = "test@autodesk.com";
            string nonInvitedDomainEmail = "test@gmail.com";

            var inviteOnlyConfig = new InviteOnlyModeConfiguration { Enabled = true, Domains = new string[] { "autodesk.com" }, Addresses = new string[] { } };
            InviteOnlyChecker checker = new InviteOnlyChecker(inviteOnlyConfig);

            bool isInvited = checker.IsInvited(invitedDomainEmail);
            Assert.True(isInvited);

            isInvited = checker.IsInvited(nonInvitedDomainEmail);
            Assert.False(isInvited);
        }

        [Fact]
        public void IsInvitedEmailTest()
        {
            string invitedEmail = "adsk.demo.tool@gmail.com";
            string nonInvitedEmail = "test@autodesk.com";

            var inviteOnlyConfig = new InviteOnlyModeConfiguration { Enabled = true, Domains = new string[] { }, Addresses = new string[] { invitedEmail } };
            InviteOnlyChecker checker = new InviteOnlyChecker(inviteOnlyConfig);

            bool isInvited = checker.IsInvited(invitedEmail);
            Assert.True(isInvited);

            isInvited = checker.IsInvited(nonInvitedEmail);
            Assert.False(isInvited);
        }

        [Fact]
        public void IsInvitedDomainsAndOtherEmailTest()
        {
            string invitedEmail = "adsk.demo.tool+0@gmail.com";
            string nonInvitedEmail = "test@gmail.com";

            var inviteOnlyConfig = new InviteOnlyModeConfiguration { Enabled = true, Domains = new string[] { "autodesk.com" }, Addresses = new string[] { invitedEmail } };
            InviteOnlyChecker checker = new InviteOnlyChecker(inviteOnlyConfig);

            bool isInvited = checker.IsInvited(invitedEmail);
            Assert.True(isInvited);

            isInvited = checker.IsInvited(nonInvitedEmail);
            Assert.False(isInvited);
        }
    }
}