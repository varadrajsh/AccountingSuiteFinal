using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Infrastructure;

public static class SqlErrorMapper
{
    public static void Map(SqlException ex, ModelStateDictionary modelState)
    {
        foreach (SqlError err in ex.Errors)
        {
            var mapped = MapParty(err)
                      // ?? MapJournal(err)
                      // ?? MapLedger(err)
                      // ?? MapInvoice(err)
                      ;

            if (mapped is (string Field, string Message))
                modelState.AddModelError(Field ?? "", Message);
            else
                modelState.AddModelError("", "An unexpected error occurred while saving data.");
        }
    }

    private static (string? Field, string Message)? MapParty(SqlError err)
    {
        var msg = err.Message;

        if (msg.Contains("UQ_Party_Name"))
            return ("Name", "A Party with this Name already exists. Please enter a unique Name.");

        if (msg.Contains("UQ_Party_GSTIN"))
            return ("GSTIN", "A Party with this GSTIN already exists. Please enter a unique GSTIN.");

        if (msg.Contains("UQ_Party_Email"))
            return ("Email", "This Email address is already registered. Please use a different Email.");

        if (msg.Contains("UQ_Party_ContactNumber"))
            return ("ContactNumber", "This Contact Number is already registered. Please use a different number.");

        if (msg.Contains("UQ_Party_Code"))
            return ("PartyCode", "PartyCode must be unique. Please try again.");

        if (msg.Contains("FK_Party_StateId"))
            return ("StateId", "Invalid State selected. Please choose a valid State.");

        return null;
    }

}

