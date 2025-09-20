using System;
using System.IO;

public class UniqueInstanceChecker {
    private const string LOCK_NAME = "FCMS_Connector_Running.lock";
    
    private FileStream lockFileStream = null;
    private static readonly string lockFilePath = Path.Combine(Path.GetTempPath(), LOCK_NAME);
    
    
    public bool isUniqueInstance() {
        if (lockFileStream != null) {
            return true; // Already locked - we're unique.
        }
        
        try {
            lockFileStream = new FileStream(
                lockFilePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None, // Block access by other processes.
                4096,
                FileOptions.DeleteOnClose | FileOptions.WriteThrough
            );
            return true; // If we're here - lock acquired!
        } catch (IOException ex) {
            // Here return fase if file already locked, and true if we don't know reason, to allow app work.
            return !isFileLockedError(ex);
        } catch (Exception) {
            // Other errors - we don't know reason, but return true to allow app work.
            return true;
        }
    }
    
    private bool isFileLockedError(IOException ex) {
        // Check if error reason is file already locked by some other instance.
        var errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(ex) & 0xFFFF;
        return errorCode == 32 || errorCode == 33; // ERROR_SHARING_VIOLATION or ERROR_LOCK_VIOLATION
    }
    
    public void Dispose() {
        lockFileStream?.Dispose();
        lockFileStream = null;
    }
    
    ~UniqueInstanceChecker() {
        Dispose();
    }
}
