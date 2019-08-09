# Web Development Wokshop Code

This repo contains the code used for my demonstrations during the Web Development Workshop I present at various conferences. It contains the final code at the state it should be at the end of the day.

## Running the application

First of all, you need to install all required npm packages in the WebDevWorkshop.Web project by running 

```bash
npm i
```

After the npm packages have been created, you can build the client-side resources by running

```bash
npm run build
```

or

```bash
npm run build:prod
```

Then you need to configure the Spotify credentials that you want to use. They can aither be set in the appSetting.json file in the WebDevWorkshop.Web project, or by setting the environment variables WDW_Spotify:ClientId and WDW_Spotify:Secret.

_Note:_ A set of Spotify credentials can be created by going to https://developer.spotify.com/.

Finally, it is just a matter of starting up all 3 projects and browsing to https://localhost:44347/.

_Note:_ If you don't have development certificates on your machine, you can add them by running `dotnet dev-certs https --trust`

## Caveats

The code is provided as-is, and I take no responsibility what so ever for what you do with it. It should work...I think... ;)

## Contact

Feel free to contact me at [@ZeroKoll](https://twitter.com) or at chris at 59north dot com

