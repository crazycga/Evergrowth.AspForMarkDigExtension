# Overview

This project was inspired by a need to embed HTML input fields into markdown documents, using the MarkDig markdown parser.  I did not find any other extension that would do what I need, and since the parsing occurs outside of Razor's parsing, I needed a way to do it through MarKDig.

# Use
Use this MarkDig extension to populate the markdown with 
```   
!ASP-FOR[<object_property_name>]
```

Doing this will parse the tag into an HTML input field, such as:

```
<input type="<based on data>" id="<name of object>" name="<name of object>" value="<value of object>" />
```

This effectively enables the programmer to populate their markdown with input boxes that conform to a data model coming from a controller.

## Configuration
The core of the configuration is an implementation of AspForGeneratorOptions, with the model attached to the "ASPModel" property.  This is then passed into the MarkDig pipeline by adding UseAspForGenerator(options).  This does mean that the programmer will need to build the pipeline for every data model that they intend to use with this plugin.

### Example configuration:

```
AspForGeneratorOptions options = new AspForGeneratorOptions();
options.ASPModel = newForm;

pipeline = new MarkdownPipelineBuilder().UseAspForGenerator(options).Build();
```

## Options
|Name|Type|Default|Function|
|---|---|---|---|
|```PopulateTagsWithModelData```|Boolean|```True```|When true, will populate the HTML with the values from the model; when false will not.|
|```DesignatePopulatedFieldsReadOnly```|Boolean|```True```|When true, will set any pre-populated field as a readonly HTML tag; when false will not.  **NOTE: this cannot be set to true when PopulateTagsWithModelData is false.**
|```NullHandling```|Enum|```NullHandling.ShowBlank```|This setting specifies what to do if the referenced model field isn't found.  The options are ```ShowBlank```, ```ShowWarning``` and ```ShowError```. (see below)|
|```WarningMessage```|String|```Field {0} not found.```|Specifies the warning message that is embedded in the markdown if the field is not found, and ```NullHandling``` is set to ```ShowWarning```.  The field {0} will return the field reference that was not found.|
|```MaxReferenceLength```|Unsigned Integer|```100```|Specifies the maximum length that the Asp-For parser will examine for a model reference name.  (see below)|
|```DateTimeOverride```|Enum|```DateTimeOverride.AsDate```|Specifies how the module handles and parses ```DateTime``` fields.  (see below)|
|```DefaultCheckedCheckboxValue``` |String| ```True``` |Specifies the default HTML value tag that is applied to checkbox type inputs.  (see below: **Class Decorators: ```AspForCheckedValue(string Value)```**|

### Null Handling
The ```AspForGeneratorOptions.NullHandling``` field controls the behavior of the rendering engine when it cannot find a model reference.  The options are:

| Option | Function |
|---|---|
| ```ShowBlank (default)``` | This will entirely eliminate the unfound reference tag in the markdown. |
| ```ShowWarning``` | This will replace the value of the unfound reference tag with the verbiage contained in the ```WarningMessage``` field.  That field will replace a {0} tag with the name of the reference field that the renderer could not find. |
| ```ShowError``` | If the renderer does not find the field in the reference tag, it will throw a new ```ArgumentNullException``` with the name of the field reference it could not find. |

### Maximum Reference Length
In order to prevent the parser from hanging if there is an unclosed !ASP-FOR[] tag, the system will stop checking after this length of string.  The default is 100 characters.  Note that since this parser will recurse through objects to find the reference, this could become quite a large number.  As an example, ```formData.ProviderCompany.Provider.FirstName``` is 43 characters long.  If the parser is running slowly, and the programmer knows the maximum length of reference names, it can be set lower; alternatively, if there are reference names longer than 100 characters, it can be set longer.

CAUTION: if the tag is completely not found during the parsing, the tag will be removed entirely from the markdown document, and might show strangely.  As an example, if the ```MaxReferenceLength``` is set to ```2``` and the line near the tag is ```"...something !ASP-FOR[MyRef] is great..."``` the result will be ```"...something Ref] is great..."```.  

### DateTime Handling
```DateTime``` types represent a problem for HTML, as HTML contains input types ```date```, ```time```, and ```datetime-local```.  The three settings below will parse **all** ```DateTime``` objects in ```ASPForm``` as the set variannt.

| Option | Function |
|---|---|
| ```AsDate (default)``` | Overrides all ```DateTime``` fields to be represented in HTML as ```date``` |
| ```AsTime``` | Overrides all ```DateTime``` fields to be represented in HTML as ```time``` |
| ```AsDateTimeLocal``` | Overrides all ```DateTime``` fields to be represented in HTML as ```datetime-local``` |

## Class Decorators
### ```[AspForIgnore] ```
In order to ignore a property in a class that is being passed to ```AspForGeneratorOptions.ASPModel``` you can use the ```[AspForIgnore```] attribute on the class.  This will cause the extension to skip over the ```!ASP-FOR[<object_name>]``` tag inside the markdown document.

**Example**

Markdown: ```This is !ASP-FOR[IgnoredAttribute] and it is good.```

```
[AspForIgnore]
public object? IgnoredAttribute { get; set; }
```

This will produce the exact same markdown as the input markdown.  The property will not be substituted.

### ```[AspForReadOnly]```
Using this decorator will designate this field with the HTML tag ```readonly``` regardless of what the ```AspForGeneratorOptions.DesignatePopulatedFieldsReadOnly``` setting indicates.  Use this to optionally desingate fields as read only at run-time, and is useful in presenting form data where some fields are pre-populated.

**Example**

```
[AspForReadOnly]
public object? ReadOnlyAttrbibute { get; set; }
```

### ```[AspForCheckedValue(string Value)]```
This decorator is used to designate on a ```Boolean``` field **only** what value should be placed in the ```value="<value>"``` field in HTML.  Check boxes have an attribute to determine the "checked" status (HTML: ```checked```) and when it is present, the ```input``` tag returns the value in the ```value``` field.  When this attribute is missing from the property, the extension will return the value in the ```AspForGeneratorOptions.DefaultCheckedCheckboxValue``` field.  Otherise, the extension will use the ```Value``` set in this attribute.

**Example**

Markdown: ```Checkbox for coffee: !ASP-FOR[DoIWantCoffee]```

```
[AspForCheckedValue("Coffee")]
public bool? DoIWantCoffee { get; set; } = true;
```

Produces: ```Checkbox for coffee: <input type="checkbox" value="Coffee" name="DoIWantCoffee" id="DoIWantCoffee" checked />```

**WARNING:** Applying this attribute to a property type that is not a ```Boolean``` will result in a run-time exception of ```ArgumentException``` to be thrown, which will also show what the property name is.

### ```[AspForRequiredInput]```
Applying this attribute to a property will set the input attribute to ```required``` in the HTML output.

**Example**

Markdown: ```Textbox for coffee size: !ASP-FOR[CoffeeSize]```

```
[AspForRequiredInput]
public string? CoffeeSize { get; set; }
```

Produces: ```Textbox for coffee size: <input type="text" value="" name="CoffeeSize" id="CoffeeSize" required />```

### ```[AspForDateTimeType(DateTimeOverride Value)]```
Using this attribute will force a ```DateTime``` field to display with the format specified by the ```DateTimeOverride``` field.  See section on DateTime Handling for options.

# Sources
|Link|Use|
|---|---|
|https://www.codeproject.com/Articles/5348000/Writing-Custom-Markdig-Extensions-2|This is a most excellent tutorial (and site) from this individual without whom I would not have been able to do this!  Thank you Mr. Richard James Moss of [Cyotek.com](https://www.cyotek.com).|
|https://www.cyotek.com/downloads/view/MarkdigMantisLink.zip|This is the sample code for the above tutorial.|
|https://stackoverflow.com/a/33481994|Narrowing down the instance for GetProperty().|
|https://stackoverflow.com/a/29823444|This AMAZING answer to use recursion to solve the problem of walking through the declared model.|
