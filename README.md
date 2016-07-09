# Object-String Map

Use a text map to turn objects into strings and back again. Good for moving structured data into and out of file paths and URLs.

## Use

- Create a class for your data. A flat POCO with simple types such as int, string, Guid and DateTime:
```
public class AccountFileKey
{
    int AccountId { get; set; }
    
    DateTime CreateTime { get; set; }
}
```
- Create a map that describes how to turn your data to and from a string. The syntax is the same as C# 6.0's new string interpolation feature:
```
var map = new StringMap<AccountFileKey>(
    "/accounts/{AccountId}/{CreateTime:yyyy/MM/dd}/data.json");
```
- Use the map to turn an object into a string:
```
var obj = new AccountFileKey
{
    AccountId = 1234,
    
    CreateTime = new DateTime(2016, 7, 9)
};

var str = map.Map(obj);

// str == "/accounts/1234/2016/07/09/data.json"
```
- Or use the map to turn a string into an object:
```
var str = "/accounts/1234/2016/07/09/data.json";

var obj = map.Map(str);

// obj should equal obj from the above example
```
