# ATS Inspect

This repository is designed to give third-parties information on how to interact with ATS Inspect services and applications.

It assumes that the user has some familiarity with Inspect. If not, a good starting point can be found here: [ATS Inspect Help](https://ats-help.com/inspect/#t=Home.htm).

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
Demonstrates creating a Winforms UserControl that can be registered via Form Composer and consumed within Data Collect. This allows extending the functionality of Data Collect in a clean and powerful way.