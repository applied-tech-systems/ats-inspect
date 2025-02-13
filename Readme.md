# ATS Inspect

This repository is designed to give third-parties information on how to interact with ATS Inspect services and applications.

It assumes that the user has some familiarity with Inspect. If not, a good starting point can be found here: [ATS Inspect Help](https://ats-help.com/inspect/#t=Home.htm).

ATS Security is an important part of the ATS ADOS ecosystem. For more info, go here: [ATS Security Manager Help](https://ats-help.com/security-configuration/#t=Home.htm). Used to manage users, roles, personal access tokens and more.

All projects are written in C# and utilize the NET 9 SDK.

More information about Inspect can be found here: [ATS Inspect](https://ats-global.com/ats-inspect).

You can get a full list of APIs by navigating to your Data Service Swagger endpoint (example: https://atsusnb191.ats-global.local:8500/swagger).

> [!IMPORTANT]
You must have ATS Security Manager, ATS Inspect Data Service and ATS Inspect Travel Service installed in order for these demos to work.

There are three projects:

### DemoApiClient
Demonstrates making API calls against our Data Service.

### DemoWebhookClient
Demonstrates registering and consuming webhook messages from Data Service.

### DemoCustomControl
Demonstrates creating a WinForms UserControl that can be registered via Form Composer and consumed within Data Collect. This allows extending the functionality of Data Collect in a clean and powerful way.