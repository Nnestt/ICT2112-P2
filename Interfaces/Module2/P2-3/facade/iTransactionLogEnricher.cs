namespace ProRental.Interfaces.Domain;

// 1. The Segregated Interfaces
public interface ILoanLogEnricher { void LogLoanProcess(int loanListId, int orderId); }
public interface IReturnLogEnricher { void LogReturnProcess(int returnRequestId, int orderId); }
public interface IClearanceLogEnricher { void LogClearanceProcess(int clearanceBatchId); }