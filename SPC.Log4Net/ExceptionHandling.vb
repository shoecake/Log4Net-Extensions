Imports System.Net
Imports System.Net.Http
Imports System.Web
Imports System.Web.Http.Filters


Namespace Exceptions

    Public Class WebAPIExceptionHandlingAttribute
        Inherits ExceptionFilterAttribute
        Public Overrides Sub OnException(context As HttpActionExecutedContext)


            If Not context.Exception Is Nothing Then
                If TypeOf context.Exception IsNot DoNotLogException Then ' leave here as not logging is when exception thrown, not when explicitly sent to logging
                    Try
                        LoggingWrapper.LogError(context.Exception, "SPCFrontEnd", "SPC unhandled error from Web API")
                    Catch
                    End Try
                End If
            End If


            If Not context.Exception Is Nothing Then
                ' format response so error shown when web.config CustomErrprs set to RemoteOnly
                ' do this in debug and release mode, if browser is remote then the correct exception error is not sent
                Dim ex = New HttpResponseMessage(HttpStatusCode.InternalServerError)
                ' keep this as data.ExceptionMessage is used in lots of places in angular code
                ex.Content = New StringContent("{""ExceptionMessage"": " & System.Web.Helpers.Json.Encode(context.Exception.Message) & "}", Text.Encoding.UTF8, "application/json")
                ex.ReasonPhrase = context.Exception.Message
                context.Response = ex
            End If

        End Sub


    End Class

    ''' <summary>
    ''' Class to call from OnApplicationError sub of Global.ASAX
    ''' none of this is umbraco specific
    ''' </summary>
    Public Class OnApplicationError

        ''' <summary>
        ''' Called from the OnApplicationError sub of Global.ASAX
        ''' Note, does not redirect to an error page (must be done by the calling code)
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Shared Sub OnApplicationError(sender As Object, e As EventArgs)


            If HttpContext.Current.IsDebuggingEnabled = False Then
                Dim ex As Exception = HttpContext.Current.Server.GetLastError()

                If Not ex Is Nothing Then
                    Try
                        ' dependency handler error - return 404 and blank message so search engines don't keep trying, also ignore the error
                        If ex.Source = "ClientDependency.Core" And ex.Message.StartsWith("no map was found for the dependency key") Then
                            HttpContext.Current.Server.ClearError()
                            HttpContext.Current.Response.Clear()
                            HttpContext.Current.Response.StatusCode = 404
                            Exit Sub
                        End If
                        If TypeOf ex IsNot DoNotLogException Then ' leave here as not logging is when exception thrown, not when explicitly sent to logging
                            LoggingWrapper.LogError(ex, "SPCFrontEnd", "SPC unhandled error")
                        End If
                    Catch
                    End Try
                End If


                ' want to render a node defined on the root node of this site (like 404 is defined)
                'Dim root As IPublishedContent = SPCUtilities.rootNode()
                'If Not root Is Nothing Then
                '    Dim errNode As IPublishedContent = root.GetPropertyValueNode("umbracoErrorPage")
                '    If Not errNode Is Nothing Then
                '        Dim errURL As New Uri(errNode.Url)
                '        Dim coll As New NameValueCollection
                '        coll.Add(SPCUtilities.HTTP_Custom_Response_Code_Key, "500")
                '        'coll.Add(SPCUtilities.HTTP_Custom_Response_Exception_Key, ex) ' can only send text strings
                '        Server.TransferRequest("~" & errURL.LocalPath, True, "GET", coll)
                '    Else
                '        ' err page is not defined for this site
                '    End If
                'Else
                '    ' error did not happen inside umbraco context, let std error be shown
                'End If

            End If
        End Sub

    End Class


End Namespace