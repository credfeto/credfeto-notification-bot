# credfeto-notification-bot

Notifications bot

## Changelog

See [CHANGELOG](CHANGELOG.md) for history

## Running a development version

1. Create a private Discord server by following the steps explained
   here: [https://support.discordapp.com/hc/en-us/articles/204849977](https://support.discordapp.com/hc/en-us/articles/204849977)
   .

2. Create a Discord Bot and invite it to your server by following the steps explained
   here: [https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token)
   .

   https://discord.com/oauth2/authorize?scope=bot&permissions=68608&client_id=YOUR_CLIENT_ID_HERE

4. Application configuration settings:

* `Discord:Token` [Required] Must be configured with the Discord Bot User Token.

4. Create a Twitch Application and authorise it:

* Create a twitch App here: https://dev.twitch.tv/console
* Authorise the twitch app https://id.twitch.tv/oauth2/authorize?response_type=code&client_id=\[Twitch API Client Id
  goes here\]&redirect_uri=http://localhost]

5.

6. Choose how you want to configure the application:

* Update `appsettings.json` inside the `Credfeto.Notification.Bot.Server` project and add the following contents:

 ```json
{
  "Database": {
    "Postgres": {
      "ConnectionString": "[Connection string to postgres DB]"
    }
  },
  "Discord": {
    "Token": "[Discord Bot User Token goes here]"
  },
  "Twitch": {
    "Authentication": {
      "UserName": "[Twitch Chat Bot Username goes here]",
      "OAuthToken": "[Twitch Chat Bot OAUth Token goes here]",
      "ClientId": "[Twitch API Client Id goes here]",
      "ClientSecret": "[Twitch API Client Secret goes here]",
      "ClientAccessToken": "[Twitch API Client Access Token goes here]"
    },
    "Channels": [
      "[Channel 1 name to react with goes here]",
      "[Channel 2 name to react with goes here]"
    ],
    "ShoutOuts": [
      {
        "Channel": "[Channel 1 name to issue shout-outs for with goes here]",
        "FriendChannels": [
          {
              "Channel": "[Channel 1 friend channel to issue shoutout for goes here - using custom shoutout format]",
              "Message": "Check out https://www.twitch.tv/[Channel 1 friend channel to issue shoutout for goes here] who streams wallpaper drying"
          },
          {
            "Channel": "[Channel 1 friend channel to issue shoutout for goes here - uses standard shoutout format]",
            "Message": null
          }
        ]
      },
      {
        "Channel": "[Channel 2 name to issue shout-outs for with goes here]",
        "FriendChannels": [
          {
            "Channel": "[Channel 2 friend channel to issue shoutout for goes here - using custom shoutout format]",
            "Message": "Check out https://www.twitch.tv/[Channel 2 friend channel to issue shoutout for goes here] who streams wallpaper drying"
          },
          {
            "Channel": "[Channel 2 friend channel to issue shoutout for goes here - uses standard shoutout format]",
            "Message": null
          }
        ]
      }
    ],
    "Raids": [
      "[Channel 1 name to issue raid welcomes for with goes here]",
      "[Channel 2 name to issue raid welcomes for with goes here]"
    ]
  }
}
```

7. Deploy the database to postgres DB. from the files in the `db` folder.
8. Build and run the `Credfeto.Notification.Bot.Server` application. You should see the bot come online in Discord and
   join the channels in twitch.

### Notes

1. The discord bot will not reply to you in the General channel!