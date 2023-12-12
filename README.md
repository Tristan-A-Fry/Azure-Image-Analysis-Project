# Azure-Image-Analysis-Project

## Description

This project was something fun to do, and to help me learn how to connect a front end to a back end while using an api to provide some additional functionality. This project was also to help me get a brief overview of ASP.NET Core and the functionality it has to offer.

## How to use

This project only runs locally and requires some brief setup (see setup & installation below). Once set up. run back end `dotnet run` and then run the frontend `npm start`.

Once up and running select an image to upload and the app should say something about the photo (note that I have not played to much with the parameters of the actual vision api, so it is very basic).
 
## Installation

- Install Azure Vision SDK https://learn.microsoft.com/en-us/azure/ai-services/computer-vision/sdk/install-sdk?tabs=windows%2Cubuntu%2Cdotnetcli%2Cterminal%2Cmaven&pivots=programming-language-csharp
- Make sure running latest version of ASP. NET Core, as well as node & react
- Must have own Azure vision key and endpoint
- Set enviroment variables with the key and endpoint
- `setx VISION_KEY yourkey` , `setx VISION_ENDPOINT yourendpoint`

## Setup

- make sure to change local host URL to your preference URL's
- backend currently on localhost:5006, frontend on localhost:3000
