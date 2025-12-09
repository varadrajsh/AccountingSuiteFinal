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
            var mapped = MapState(err)
                        ?? MapAudit(err)
                        ?? MapParty(err)
                        ?? MapAccountHead(err)
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

    // State-specific mapping
    private static (string? Field, string Message)? MapState(SqlError err)
    {
        var msg = err.Message;

        if (msg.Contains("UQ_State_StateName"))
            return ("StateName", "A State with this Name already exists. Please enter a unique State.");

        if (msg.Contains("FK_State_RegionId"))
            return ("RegionId", "Invalid Region selected. Please choose a valid Region.");

        return null;
    }
    // Audit Model
    private static (string? Field, string Message)? MapAudit(SqlError err)
    {
        var msg = err.Message;

        // Unique constraint on AuditId (should rarely happen)
        if (msg.Contains("PK_TransactionAudit"))
            return ("AuditId", "Audit record could not be saved due to a duplicate key issue.");

        // Module must be valid
        if (msg.Contains("CK_TransactionAudit_Module"))
            return ("Module", "Invalid module specified for audit. Please check the transaction source.");

        // TransactionId foreign key issues
        if (msg.Contains("FK_TransactionAudit_Transaction"))
            return ("TransactionId", "Invalid TransactionId. The referenced record does not exist.");

        // Branch foreign key issues
        if (msg.Contains("FK_TransactionAudit_Branch"))
            return ("BranchId", "Invalid Branch selected. Please choose a valid branch.");

        // Action must be valid
        if (msg.Contains("CK_TransactionAudit_Action"))
            return ("Action", "Invalid action specified. Please check the operation type.");

        // Status must be valid
        if (msg.Contains("CK_TransactionAudit_Status"))
            return ("Status", "Invalid status specified. Please check the transaction result.");

        // Approval/Disapproval foreign key issues
        if (msg.Contains("FK_TransactionAudit_ApprovedBy"))
            return ("ApprovedBy", "Invalid approver selected. Please choose a valid user.");

        if (msg.Contains("FK_TransactionAudit_DisapprovedBy"))
            return ("DisapprovedBy", "Invalid disapprover selected. Please choose a valid user.");

        return null;
    }

    //Party Model
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

    //Account Head Model
    private static (string? Field, string Message)? MapAccountHead(SqlError err)
    {
        var msg = err.Message;

        if (msg.Contains("UQ_AccountHead_Name"))
            return ("AccountHeadName", "An Account Head with this Name already exists. Please enter a unique Name.");

        if (msg.Contains("UQ_AccountHead_Type"))
            return ("AccountHeadType", "This Account Head Type already exists. Please choose a different Type.");

        if (msg.Contains("UQ_AccountHead_NameTypeLookup"))
            return ("AccountHeadName", "Duplicate entry: An Account Head with the same Name, Type, and Lookup already exists.");

        if (msg.Contains("FK_AccountHead_Parent"))
            return ("ParentAccountHeadId", "Invalid Parent Account Head selected. Please choose a valid parent.");

        return null;
    }

}

