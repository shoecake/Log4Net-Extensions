Imports System.Web
Imports log4net
Imports SPC.Log4Net.Exceptions

Public Class LoggingWrapper
    Private Shared Function ObjectToString(obj As Object) As String
        Try
            If obj Is Nothing Then
                Return ""
            Else
                Return obj.ToString
            End If
        Catch
            Return ""
        End Try
    End Function



    Public Shared Sub LogError(lastError As System.Exception, Optional LogType As String = "SPCFrontEnd", Optional logMessage As String = "SPC unhandled error")
        Dim Log As ILog = LogManager.GetLogger(LogType)

        ' add in httpcontext info
        GlobalContext.Properties("URL") = ObjectToString(HttpContext.Current.Request.Url)
        GlobalContext.Properties("ReferrerURL") = ObjectToString(HttpContext.Current.Request.UrlReferrer)
        GlobalContext.Properties("UserAgent") = ObjectToString(HttpContext.Current.Request.UserAgent)
        GlobalContext.Properties("UserName") = ObjectToString(HttpContext.Current.User.Identity.Name)

        If lastError Is Nothing Then
            Log.Error(logMessage)
        Else
            Log.Error(logMessage, lastError)
        End If
    End Sub


    Public Shared Sub LogError(lastError As String, Optional LogType As String = "SPCFrontEnd")
        LogError(Nothing, LogType, lastError)
    End Sub

    Public Shared Sub LogErrorAndRaiseException(lastError As System.Exception, LogType As String)
        LogError(lastError, LogType)
        Throw New DoNotLogException(lastError.Message)
    End Sub


    Public Shared Sub Log404Error()
        Dim Log As ILog = LogManager.GetLogger("SPC404")

        ' add in httpcontext info
        GlobalContext.Properties("URL") = HttpContext.Current.Request.Url.ToString()
        If Not HttpContext.Current.Request.UrlReferrer Is Nothing Then
            GlobalContext.Properties("ReferrerURL") = HttpContext.Current.Request.UrlReferrer.ToString()
        Else
            GlobalContext.Properties("ReferrerURL") = ""
        End If
        Log.Error("404 Page not found")
    End Sub


End Class
