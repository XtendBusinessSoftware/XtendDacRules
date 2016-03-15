# XtendDacRules
This project contains a number of extra code analysis rules for SSDT.
The extra rules are:

* XR0001: All foreign keys should have an index that covers the foreign key
* XR0002: All tables should have a description
* XR0003: All columns should have a description
* XR0004: All procedures must set nocount
* XR0005: No unused variables may exist
* XR0006: No unused parameter may exist
* XR0007: All mandatory parameters must be filled in
* XR0008: Avoid setting concat_null_yields_null option to off

The wiki contains a quick guide how to install the rules and a short description of the included rules.

If you do not want to compile the dll containing the rules yourself, have a look at the releases. It has a zip (called XtendDacRules.zip) containing the compiled dll.

IMPORTANT: If you use the zip with the compiled dll, check the properties of the dll before starting Visual Studio. The dll might be blocked and in that case needs to be unblocked before you can use it.


