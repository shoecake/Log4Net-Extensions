Imports log4net.Core
Imports log4net.Filter
Imports SPC.Extensions.Common

Namespace Filters

#Region "Sample XML"
    '<filter type = "SPC.Log4NetFilters.RateLimitFilter, SPCLog4NetFilters" >
    '<exceptionClass>
    '    <className value="AutoLoginSecurityException"></className>
    '    <timePeriod value="60"></timePeriod>
    '    <threshold value="5"></threshold>
    '</exceptionClass>
    '<exceptionClass>
    '    <className value = "GetAnonymousQuoteSecurityException" ></className>
    '    <timePeriod value = "60" ></timePeriod>
    '    <threshold value = "5" ></threshold>
    '</exceptionClass>
    '<acceptOnMatch value = "false" />
    '</filter>
#End Region

    Public Class iClass
        Public Property className As String
        Public Property timePeriod As Integer
        Public Property threshold As Integer
    End Class

    ''' <summary>
    ''' Blocks errors unless X nos occur within a set time period.
    ''' Errors are grouped by the class of the exception 
    ''' </summary>
    Public Class RateLimitFilter
        Inherits FilterSkeleton
        Private m_acceptOnMatch As Boolean
        Private classes As New Dictionary(Of String, iClass)


        Public Overrides Function Decide(loggingEvent As LoggingEvent) As FilterDecision
            Static RateOfEvents As New Dictionary(Of String, Queue(Of Date))
            Dim className = loggingEvent.ExceptionObject.GetType.ToString()

            If classes.Keys.Contains(className) Then ' if exception is not in the list then allow to pass through
                Dim rateItem As Queue(Of Date)
                Dim classItem = classes(className)
                Dim dateOfLastMessage As Date = Now.AddMinutes(-1 * classItem.timePeriod)

                If RateOfEvents.Keys.Contains(className) Then
                    rateItem = RateOfEvents(className)
                Else
                    rateItem = New Queue(Of Date)
                End If

                rateItem.Enqueue(Now) ' ad this item to thre queue
                If rateItem.Count > 0 Then
                    Do While rateItem.Peek < dateOfLastMessage ' remove any items which are now past the time period
                        rateItem.Dequeue()
                    Loop
                End If

                RateOfEvents.AddOrUpdate(className, rateItem)
                If rateItem.Count >= classItem.threshold Then
                    loggingEvent.Properties.AddOrUpdate("SPCRateLimitMessage", "The exception of class " & className & " has occurred " & rateItem.Count & " times in the last " & classItem.timePeriod & " minute(s). All errors can be seen in the main logs. The last error message was:")
                    loggingEvent.Fix = FixFlags.Properties ' need to save the new properties
                    Return FilterDecision.Neutral
                Else
                    Return FilterDecision.Deny
                End If
            Else
                ' pass through error
                loggingEvent.Properties.AddOrUpdate("SPCRateLimitMessage", "") ' so null is not shown in the message
                loggingEvent.Fix = FixFlags.Properties
                Return FilterDecision.Neutral
            End If


        End Function




        Public WriteOnly Property exceptionClass() As iClass
            Set(value As iClass)
                If value.className IsNot Nothing AndAlso value.className <> "" Then
                    If value.timePeriod = 0 Then
                        value.timePeriod = 60
                    End If
                    If value.threshold = 0 Then
                        value.threshold = 10
                    End If

                    classes.Add(value.className, value)
                End If
            End Set
        End Property


    End Class




End Namespace
