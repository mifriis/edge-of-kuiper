# Contributing to Edge-of-Kuiper

Head over to: https://github.com/mifriis/edge-of-kuiper/issues?q=is%3Aopen+is%3Aissue+label%3A%22Up+for+grabs%22 

There we have a list of things you can contribute with.

Create an issue if you are experiencing problems or have feature requests.

Branch/Fork & PR to contribute back in main.

## Making great code additions

### Commands

The ones players input into the terminal. They are UI code and should not be UnitTested.

A good command is:

* Part of a command group that makes sense. Avoid making new groups unless you are certain it should be one.
* Using the Console for input and output, but it's logic should never go beyond what and how to deal with the UI.
* Using Services for business logic.
* Trying to make input for the user as easy as possible. If there's a list of choices, present it with an easy followup for the choice. Don't rely on copypasting or excessive typing.
* Calling methods in Services.
* Not creating events.

### Events

Things that happened while the player was logged out. They can require input (choices), have multiple outcomes or be part of a chain.

A good event is:

* Part of a game feature.
* Soon! The players will login maybe once a day, they don't want to wait, real time, weeks or months for an event to happen.
* Unit Tested, but not the console input and output. Verify service calls are done.

### Services

The core of EOK, here's where most of the logic is contained if it doesn't go directly into the Domain Object. They support Domain Objects and Game Features.

A good service is:

* Part of a GameSystem and a Domain.
* Responsive and avoids voids. Think about what makes sense to send back to the command or event to avoid pushing logic down there.
* Completely UnitTested (100%), yes even that super simple feature.

