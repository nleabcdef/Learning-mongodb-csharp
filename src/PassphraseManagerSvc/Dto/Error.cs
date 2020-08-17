using System;
namespace PassphraseManagerSvc.Dto
{
    public class Error
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }

    public class PassStoreException : Exception
    {
        public ErrorCategory Category {get; set;}
        const  string _msg = "Internal Server Error";
        public PassStoreException(Exception ex)
            :base(ex == default(Exception)?_msg: ex.Message, ex)
        {
            Category = ErrorCategory.ProgramError;            
        }
        
        public PassStoreException(ErrorCategory category, string message, Exception ex =null)
            :base(message?? _msg, ex)
        {
            Category = category;

        }
        public enum ErrorCategory
        {
            InvalidInput = 1,
            Configuration,
            ProgramError,
            ServerError,
            DatabaseError,
            Others
        }
    }
}