# Log4Net Extensions

This library contains a number of helper function for log4Net


 
## Filters

### And Filter
This is a simple filter which can be used to combine 2 or more filters into an AND clause. 

### Rate Limit Filter
This filter can be used to "rate limit" the number of errors you will get logged. 

If you have an intermittent error which you are aware of and don't mind if this is happening infrequently, but you do want to know if suddenly a lot of these errors occur in a short time, then this filter is for you.
An example is if you have code which communicates with a third party which is not always available. You can specify how many errors in a time period you wish to ignore and if the amount of errors exceeds this then the filter will pass the error through to the logger along with information on how many errors there have been in that time period.
The errors to be monitored by this filter are grouped by the exception class.


