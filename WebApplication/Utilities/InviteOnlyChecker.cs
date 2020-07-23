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