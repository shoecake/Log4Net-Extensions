Imports log4net.Core
Imports log4net.Filter



Namespace Filters


#Region "Sample XML"
    '<filter type = "SPC.Log4NetFilters.AndFilter, SPCLog4NetFilters" >
    '  <filter type="log4net.Filter.PropertyFilter">
    '      <key value="URL"/>
    '      <stringToMatch value="/foo/foobar.aspx"/>
    '  </filter>
    '<filter type = "log4net.Filter.PropertyFilter" >
    '  <key value="UserID"/>
    '<stringToMatch value = "TESTUSER" />
    '  </filter>
    '<acceptOnMatch value = "false" />
    '</filter>
#End Region



    ''' <summary>
    ''' All of the filters are true
    ''' </summary>
    Public Class AndFilter
        Inherits FilterSkeleton
        Private m_acceptOnMatch As Boolean
        Private ReadOnly filters As IList(Of IFilter) = New List(Of IFilter)()

        Public Overrides Function Decide(loggingEvent As LoggingEvent) As FilterDecision
            If loggingEvent Is Nothing Then
                Throw New ArgumentNullException("loggingEvent")
            End If

            For Each filter As IFilter In filters
                If filter.Decide(loggingEvent) <> FilterDecision.Accept Then
                    Return FilterDecision.Neutral
                    ' one of the filter has failed
                End If
            Next

            ' All conditions are true
            If m_acceptOnMatch Then
                Return FilterDecision.Accept
            Else
                Return FilterDecision.Deny
            End If
        End Function

        Public WriteOnly Property Filter() As IFilter
            Set(value As IFilter)
                filters.Add(value)
            End Set
        End Property

        Public Property AcceptOnMatch() As Boolean
            Get
                Return m_acceptOnMatch
            End Get
            Set(value As Boolean)
                m_acceptOnMatch = value
            End Set
        End Property
    End Class


End Namespace