# String Map

Use a text map to turn simple objects into strings and back again. 

Good for moving structured data into and out of file paths and URLs.

## Usage

Create a class for your data. A flat POCO with simple types such as `int`, `string`, `Guid` and `DateTime`:
```
public class FileKey
{
    int AccountId { get; set; }
    
    DateTime? CreateTime { get; set; }
}
```
Create a new `StringMap` that describes how to turn your data to and from a string:
```
var map = new StringMap<FileKey>(
    "/accounts/{AccountId}/{CreateTime:yyyy/MM/dd}/data.json");
```
Use the `StringMap` to turn an object into a `string`:
```
var obj = new FileKey
{
    AccountId = 1234,
    
    CreateTime = new DateTime(2016, 7, 9)
};

var str = map.Map(obj);
```
Here `str` will be `/accounts/1234/2016/07/09/data.json`.

Use the `StringMap` to turn a `string` into an object:
```
var str = "/accounts/1234/2016/07/09/data.json";

var obj = map.Map(str);
```
Here `obj` will equal the `FileKey` defined above.

## Partial Mapping

Suppose in the above example that you have an `AccountId` but you do not know the `CreateTime`:
```
var fileKey = new FileKey
{
    AccountId = 123,
    
    CreateTime = null // or just omit this for the default of null.
};
```
You can create a partial string that will be as complete as possible from left to right, stopping once a needed value is null:
```
var str = map.Map(fileKey, allowPartialMap: true);
```
Here `str` will have a value of `/accounts/1234/`.
