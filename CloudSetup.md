# Setup for Cloud deploy

To get this to run in Azure you will need to do some things but only once.
For this initial setup it doesn't really make sense to have a GitHub action
to setup the azure resources.

To help minimize error we will use the Azure CLI. The easiest way to
leverage the commands are to login to the [azure portal](https://portal.azure.com/)
and click the _Cloud Shell_ icon in the top right corner of the navigation.

## Create Resource Group

We will create an Azure Resource Group. You can name it whatever you want,
the name has to be unique in the Azure Subscription but is not a global
name. There can be hundreds of people each with their own
`orleans-helloworld-rg` resource group. You can also select an appropriate
azure region for you.

```
az group create --name orleans-helloworld-rg --location westus3
```

Copy the "id" part of the json payload that returns the id should be:

```
/subscriptions/<sub-guid>/resourceGroups/orleans-helloworld-rg
```

## Create Web Plan

We will need a Web Server to provide the communication bridge between the
Silos (i.e. Orleans Server Nodes) and the Clients. To start we will create
the minimal deployment of a Web Server. The _"Plan"_ which describes how
Azure will provision your Web Server. The plan is actually an Azure service
that hosts other services besides Web Servers. You should adjust the azure
region, for consistency, the same region as the resource group is
recommended.

```
az appservice plan create --resource-group orleans-helloworld-rg --name orleans-helloworld-plan --is-linux --sku F1 --location westus3
```

You should get a json payload with the various information about the plan
if the plan was created successfully.

## Create Web Server

Next we will need to create the actual Web Server (i.e. _"Web App"_). Here
the name provided for the Web App is provided must be unique globally.
Since this is going to be a dot-net-core 8 WebApp, we will specify that
string. If this is the future and you want to use a newer version, you can
use the following Azure CLI command to see what string is appropriate.

```
az webapp list-runtimes
```

Run the following command to create the web server.

```
az webapp create --name talonseverhost --resource-group orleans-helloworld-rg --plan orleans-helloworld-plan --https-only --runtime "DOTNETCORE:8.0"
```

## Create Service Principal

Create the Azure Service Principal to grant access to GitHub actions so it
has the appropriate security to create the resource in Azure. First, the
_"user"_ account will be created with the following command. It is **very
important** to grab the `password` value after you create the service
principal as it isn't stored anywhere and there is no way to retrieve it.
You can change the `--display-name` if you want or not even provide one.

```
az ad sp create-for-rbac --display-name "orleans-helloworld-deploy"
```

Copy and paste the entire json result as it will be used later when
configuring secrets. As mentioned before, it is **extremly important** that
you save the `password` as this is the only time it is shown.  Here is an
example of what the json looks like.

```
{
  "appId": "<unique-guid-for-this-service-principal>",
  "displayName": "orleans-helloworld-deploy",
  "password": "<the-secret-password-you-must-save>",
  "tenant": "<guid-for-your-entra>"
}
```

## Assign Role to the Service Principal

The Service Principal _"user"_ account is now created, but it doesn't have
any rights to do anything. Next we will assign _Roles_ to the Service
Principal so it will have the correct permissions.

```
az role assignment create --assignee "<appId>" --role "Website Contributor --scope "/subscriptions/<subID>/resourceGroups/orleans-helloworld-rg"
```

The `<appId>` is the guid that was specified in the previous step when the
Service Principal was created. `Virtual Machine Contributor` is a
predefined role that grants a set of permissions. Scope is required and in
this example it is limited to a specific resource group.

## Configure GitHub Secret

On the GitHub repo for this project, navigate to the _Settings_ tab and
then select [Secrets and variables/Actions](settings/secrets/actions).
Click the _"New repository secret"_ button to create the encrypted secret.
For use with the configured GitHub actions you will need to name the secret
`AZ_DEPLOY_CRED`. The json that has to be saved as the secret is slightly
different then what is needed.

| Az Cli Key | GitHub Action Key |
| ---------- | ----------------- |
| appId      | clientId          |
| password   | clientSecret      |
| tenant     | tenantId          |
| (none)     | subscriptionId    |

The `subscriptionId` wasn't returned but it is embedded in the resource
group ID created in the first step. The value that goes in GitHub should
look similar to:

```
{
  "clientSecret": "<the-secret-password-you-must-save>",
  "subscriptionId": "<sub-guid>",
  "tenantId": "<guid-for-your-entra>",
  "clientId": "<unique-guid-for-this-service-principal>"
}
```