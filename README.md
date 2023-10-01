<div align="center">
  <h1><img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/ade_logo.png" width="35px" height="35px" style="margin-bottom: -3px;" /> Aurora Dialog Enhancer</h1>
</div>

<p align="center">
    【English | <a href="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/README-Russian.md">Русский</a>】
</p>

**ADE** - is an application that extends the functionality of dialog interactions in various games.

Most games have the ability to select dialog options using mouse wheel or keyboard buttons, but some games either don't have this feature or it is implemented poorly.
This application fixes this issue by means of extensions, individually developed for each game, thus extending the game functionality.
Also, usage of this software cannot cause your account to be banned in any games you play *([more in this section](#%EF%B8%8F-how-it-all-works))*.

<div align="center">
  <img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/ade_ingame_example.gif" />
  </br>
  <i>Now, by using a keyboard and mouse wheel, dialog options can go brrrrrrrrrrrrrrrr</i>
</div>

## ❓ Why and what for? Using Genshin Impact as an Example
- Why can't you choose a dialog option using mouse wheel, the same way you would when selecting which item you want to place in your inventory?
- Why can you pick up an item with the <code>f</code> button, but the dialog can <b>only</b> selected with the mouse cursor?
- Why is the cursor always in the center of the screen at the beginning of dialogs and cutscenes?
- Why can't you take screenshots in cutscenes and dialogs?
  
> If you have ever asked such questions, then this application is for you.

## 📺 [YouTube](https://youtube.com/video/ch2GWBYqNSE) usage demonstration video
<div>
  <a href="https://youtube.com/video/ch2GWBYqNSE" target="_blank"><img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/youtube_thumbnail.jpg" alt="Aurora Dialog Enhancer | Long demonstration" />
</div>

## 🖼️ Screenshots
<div>
  <img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/app_screenshot_01.jpg" />
  <img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/app_screenshot_02.jpg" />
</div>

## 📖 Key features

* Binding any keyboard and mouse key combinations to various actions
* Interacting with dialogs using key bindings
* Confirming dialog options
* Cycling through dialog options
* Cursor hiding
* Screenshots saving
* And much more

## 💻 System requirements

* Windows 7 (x64) and up
* Administrator privileges *([see here as to why](#-why-are-administrator-privileges-required))*
* 2 MB of free space
* .NET Framework 4.8 *([built in Windows by default](https://learn.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies#net-framework-48))*

## ✨Setting up and usage

As the development progressed, more and more features were added to the application, and although everything was aimed at providing the most convenience, now there is so much functionality that it became necessary to document it here.

The application is modular, and works with extensions individually developed for each game. Each extension is treated as a game profile, allowing you to fine-tune it.

To get it all going just [**download the application**](https://github.com/DigitalE4rth/AuroraDialogEnhancer/releases), run it and hit a big fancy `Start` button. That's it.

And if you want to know more about other features, you can check them out below.

### 🎣 Hook control

`Hook` - is the corner stone of the application. In order to work, the application needs to find the running game process and attach to it. To do this, you need to select required `Game Profile` and specify game process name. 

In most cases, extension developers set everything up, but you can customize all the settings yourself.

* `Start` is the most important button of the `Profile` subsection. When you click on it, the launch action you selected in the `Advanced settings` subsection is performed. If you want the `Game` or `Launcher` of the game to be launched along with it when the hook starts, change this setting.
* When the hook is launched, the application will begin searching for the game process by its specified name, and if it finds one, it will connect to it.
* To avoid specifying all the paths manually, you can use `Locate` option in profile `Drop-down menu`.
* `Create shortcut` - creates an application shortcut to quickly launch the hook of selected `Game Profile`, with all specified settings.
* In the same section you can set the directory for screenshots and `Exit with the game` setting.

### ⌨️ Key bindings

In this section individual settings are specified for each `Game Profile`. Default key bindings are set by extension developers. You can `Reset` or `Clear` those settings in `Drop-down menu`.

#### Action Settings

* `Hide cursor on manual selection` - after detecting dialog options, clicking on one of them will hide the cursor.
* `Cycle through dialog options` - switching to the first/last dialog options when selecting the next/previous one at the end and the beginning of the list, respectively.
* `Single dialog option behavior` - dialog selection confirmation when only one option is present. You can highlight or select it without confirmation.
* `Numeric action behavior` - confirmation of dialog choices selected by pressing number keys *(e.g. 1, 2, 3)* by highlighting them, or by selecting them without confirmation.
* `Cursor behavior` - hide after any action in a dialog, hide after a selection, or do nothing.

#### General

* `Pause/Resume Hook`
* `Reload hook` *(necessary when changing game resolution, and repeating the search for dialog options)*
* `Take screenshot`
* `Hide cursor`

#### Controls

* `Select the dialog option`
* `Select previous`
* `Select next`

#### Selecting a dialog option

* `Quick selection with numeric keys`

#### Additional extension actions

In extensions it is possible to specify additional actions - points on the game screen where you can click to perform certain action during a dialog.
For example, in Genshin Impact, the `Autoplay` action has been added - by clicking on the `> Auto` button in the upper left corner of the screen in a dialog.

### 🖼️ Appearance

The application is developed with C# programming language using WPF technology. Using this technology, the author of the application developed a library of custom components and styles [*Why Orchid*](https://github.com/DigitalE4rth/WhyOrchid), which allowed not only to improve the graphical components of the application, but also to increase accessibility by implementing the following functionality:

* Selecting and creating of color schemes
* Changing of accent color
* Scaling
* Selecting of custom font
* Selecting of interaction cursor view

### ⚙️ Settings

* Language setting
* Window state at startup (`Default`, `Taskbar`, `System tray`)
* Window state when launching profile shortcut (`Default`, `Taskbar`, `System tray`)
* Minimizing instead of closing
* Minimizing to taskbar
* Launching on Windows startup
* Opening a screenshots directory *(if taken)* when exiting the game
* Expert mode *(allows to fine-tune the internals of the application)*
* Opening the application folder and application settings folder

### 🎆 About the program

This section contains all the information about the program and its developers, as well as update mechanisms and notifications.

## 🎇 About the project

Everything you wanted to know but were afraid to ask 🤔

### 🎮 Supported Games

This is an OPC (One Person Company), so the developer works alone. Thus everything will be implemented, but not right away.

#### ⌚ Works like clockwork

* Genshin Impact

#### 🧑‍💻 In developing

* Star Rail
* Wuthering Waves
* Project Mugen
* Tower Of Fantasy
* Zoneless Zone Zero

### 🕹️ How it all works

Usually, when developing cheats and scripts for games (*and this application is not a cheat!*) certain data is found and used in the game memory. To prevent this, online game developers make anti-cheats.
Some only respond to obviously unintended game behavior by allowing memory reads, while some also monitor memory reads from their games.

It would have been ten times easier to implement such an approach, adding a injectable library as well.
But since this violates license agreement of some games, and there is a risk of getting banned, the application itself was developed to function as an intermediary that works with extensions, where the developers of those take responsibility for it.

Since each game is unique in its protection, each extension uses its own approach to bypass these limitations (*if any*), for example, Genshin Impact uses on-screen image processing.
Therefore, for this game it is absolutely (*99.9%*) safe.
Can you get banned in a game for taking a screenshot?
No. The developer has been using and testing this application for over a year.
He gives you his quality assurance - everything possible was done for it to be legal, not violating game's license agreement.
In addition, use of scripts like [**AutoHotKey**](https://www.autohotkey.com) is allowed in almost all games.

> <details>
> 	<summary>How it works in detail for Genshin Impact</summary>
>   </br>
>   <div>
>     <p>First we need to determine whether the game is in dialog mode. To do this, each time a registered action is performed in the program, we check whether the cursor is displayed (and when characters move, it is not displayed), whether an NPC is speaking to the character (for this we take a screenshot of a small area at the bottom of the game's window, and we determine the presence of NPC name by unique color range), and only after that a screenshot is taken of the limited area in which the dialog bubbles should be located. Then they are compared with a mathematically specified template.</p>
>     <p>Having taken screenshots of all possible screen resolutions in the game, patterns were derived to mathematically define where dialog bubbles are located, i.e. how the size of the game window defines their location and size. After that, the template is then programmatically set based on the contour lines of the dialog bubbles, brightness transition, icon presence, etc.</p>
>     <p>Long-term testing showed that this approach works for all game resolutions, starting from 800x600, and that the dynamically calculated formula gives a match result of 98%.</p>
>     <p>Which means that in incredibly rare cases the application may find an extra dialog option or miss an existing one. If this happens, all you have to do is press <code>f5</code> to perform the search again. It is also worth knowing that the search does not work with an already highlighted dialog option, i.e. when the mouse cursor is on it.</p>
>     <p>The developer and testers were happy with the end result, I hope you will be happy with it too.</p>
>   </div>
> </details>

**See also**: [*Why I had to give up on OpenCV*](https://github.com/DigitalE4rth/AuroraDialogEnhancer/discussions/7)

### 🔑 Why are administrator privileges required?

This application intercepts keyboard and mouse events to respond to their click events, but due to Microsoft [restrictions](https://learn.microsoft.com/en-us/previous-versions/dotnet/articles/bb625963(v=msdn.10)?redirectedfrom=MSDN#user-interface-privilege-isolation-uipi-and-integrity), applications with lower privileges cannot intercept events of applications with higher privileges. And since Genshin Impact only works with administrator privileges, application privileges must match. Nothing you can do about this.

### 🗺️ Development roadmap

The roadmap can be found [*here*](https://github.com/users/DigitalE4rth/projects/1).

## 💬 Let's talk about important things

You've reached the last sections, congratulations!🥳
There's plenty to read here if you're interested in developer's thoughts and development path.

### 🛣️ The pathway

It was... A very long journey. I started developing this application purely for Genshin Impact, and solely for myself, because it was inconvenient for me to drink tea 🍵, eat cookies 🍪, read dialogs 💬 and move the mouse 🖱️ at the same time.
Yes, programmers are very lazy.
That’s why the development took... Almost a year 🥲.
I started this project in early November 2022, and finished it in September 2023...
I'm surprised that I got this to release, really... This is an achievement...
I had been coding this application in evenings almost every day after finishing with my work
(*well... and during work hours too. Something I’m not very proud of* 😓).

What else? No personal life, minimum entertainment, a ton of time and effort spent on this project...
I can’t say what kind of response this will yield in the future, but as a programmer I have grown quite well.

This the third project out of ten that I started, that has saw the release.
There are so many ideas, but very little time and energy... + I couldn’t have done it without the help and support of all my beloved friends, so a huge thank you to you all 💖

> <details>
>   <summary>Well, I hope you enjoy it, i did my utmost best...</summary>
>   <div align="center">
>     <img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/i_did_my_besht_meme.jpg" />
>   </div>
> </details>

### 🥰 Acknowledgements

* [**Meowmaniac**](https://github.com/Meowmaniac) - Testing, criticism. Thanks for listening to me whining and constantly testing all the janky code. Sorry for the bug that took up all the space on your hard drive)))0) /^._.^\ฅ🔪
* [**NDS**](https://github.com/nestdimon) - Criticism, architectural consulting, whine listening. Thank you for your time, opportunities and support. Thank you, I mean it... (((*°▽°*)八(*°▽°*)))♪
* **Tortuga** - Testing, whine listening. Thank you for testing the program at least at the very end of development) Someday we will all get together again and play DnD... Someday (⁠ ⁠≧⁠Д⁠≦⁠)🏰🎲🐲(⁠≧⁠▽⁠≦⁠)
* [**Vobraz**](https://github.com/Vobraz) - OpenCV consulting, criticism. This guy even rebuilt OpenCV for me, reducing the original size of the program from 60 MB to 24 MB... I asked him a lot of questions to understand the intricacies of the work, but in the end, having studied everything, I wrote my own super lightweight version of OpenCV, which is why the application size became... 2 MB. Well... It happened) Anyway, thanks, you're awesome! 🤩💯🔥ヾ(⌐■_■)ノ♪
* [**WertQj**](https://github.com/WertQj) - English translator and editor, translation quality control. Perhaps, as you said, I could handle everything myself, but it would have taken much longer. ↜₍^  -༝-^₎ In any case, thank you, and not only for the translation, friend! 
* [**ZloeZlo**](https://github.com/zloezzlo) - Ukrainian translation, translation quality control. I know it was a joke *(translation into Ukrainian)*, but you still managed it. Thank you) .⭐ ˖🦉‧₊˚ ☾.
* Special thanks to **all of you**, users of this application, for your use, criticism, feedback and discussion. This is very valuable. You are the best, I love you all.
* （づ￣3￣）づ╭❤️～

#### 🦄 Special thanks

* Microsoft - for severely restricting my previous GitHub account just because I live, in their opinion, in the *wrong place* >:(
* Gitee - for deleting my account after half a year without any warning.
* Microsoft once again - for having to use GitHub with VPN, and also for dropping support for the .NET Framework and WPF, and also for the fact that you can’t roll back from Windows 11 to Windows 10.
* JetBrains - Because they haven’t been able to fix [XAML designer for WPF](https://youtrack.jetbrains.com/issue/RIDER-81870/Wpf-Xaml-offset-between-clickable-elements-and-rendered-objects) for a year now.

### 🛠️ Libraries and resources

Although the application uses all the libraries listed below, at least 80% of each library had to be rewritten to optimize them for specific tasks, reducing the space they take up as well...
Very little remains from the original versions, but, none the less, I would like to thank the developers for their work, ideas and open source code.

* [**AutoUpdater.NET**](https://github.com/ravibpatel/AutoUpdater.NET) - I don't know guys, it doesn't seem to me that using WebView2 is necessary for your library. And why is everything `static` everywhere? I don't understand... But thanks anyway.
* [**NonInvasiveKeyboardHook**](NonInvasiveKeyboardHook) - A unique idea of setting, tracking and responding to key combinations. Very cool, but I rewrote everything for my needs, + added support for mouse buttons. Thank you.
* [**Why Orchid**](https://github.com/DigitalE4rth/WhyOrchid) - Oh, this one's mine. Pure WPF, nothing extra, which makes it very lightweight. I want to rewrite most of it in the future, so I don’t recommend relying on its release versions - edit the code yourself if you decide to use it.
* [**WPF Color Picker**](https://github.com/dsafa/wpf-color-picker) - A powerful color picker. So UWP has a built-in one, but WPF doesn’t? Thanks again Microsoft. To the author for this lib as well, even though I rewrote everything).
* [**Google Fonts**](https://fonts.google.com/icons) - SVG icons and pictograms.
* [**Font Awesome**](https://fontawesome.com) - SVG icons and pictograms.
* [**Application icon**](https://svgmix.com/item/zQWr2M/aurora) - Northern lights.

## 💖 Creative society

One of the weighty and important reasons why the application was released was to notify as many people as possible about [**Creative Society**](https://creativesociety.com) project.
I would not have put as much effort into improving the usability and quality of the application if I had not counted on its popularization and distribution...
What is CS and why is it so important not only for me, but for everyone in general? Well, the answer is simple:

CS is an international project, the goal of which is to legally and peacefully transition to a new creative format of society throughout the world in the shortest possible time, in which human life will be the highest value.

Why is it important for me and you?
This is important for me because for a third of my life now I have been living in a war zone with no borders, where nothing foreshadows its end, and where there is no value for human life.
For you, just as for me, CS is that global way out of all crises, and not just wars, it is also a way to fight natural disasters.

There is a saying: 
> "If a scientist cannot explain to a 5-year-old child what he does, then he is a fraud."

The goal of Creative Society is to bring human life to the forefront legislatively, by adopting 8 principles in all countries of the world that will support and ensure that:

<details>
	<summary><b>8 Pillars of the Creative Society</b></summary>
  </br>
  <b>The 8 Pillars of the Creative Society are what people from all over the world desire. These are the fundamental values of the Creative Society that can become the basis of international law and legislation in all countries through a lawful expression of people's will at the world referendum.</b>
  <div>
    <h3>1. Human Life</h2>
    <p>Human life is the highest value. Life of any Human has to be protected as one's own. The goal of society is to ensure and guarantee the value of each Human's life. There is not and never can there be anything else more valuable than a Human's life. If one Human is valuable, then all People are valuable!</p>
  </div>
  <div>
    <h3>2. Human Freedom</h3>
    <p>Every human is born with the right to be a Human being. All People are born free and equal. Everyone has the right to choose. There can be no one and nothing on Earth superior to a Human, his freedom and rights. The implementation of Human rights and freedoms must not violate the rights and freedoms of others.</p>
  </div>
  <div>
    <h3>3. Human Safety</h3>
    <p>No one and nothing in society has the right to create threats to the life and freedom of a Human!</p>
    <p>Every Human is guaranteed free provision of essential life necessities, including food, housing, medical care, education and full social security.</p>
    <p>Scientific, industrial and technological activities of the society should be aimed exclusively at improving the quality of human life.</p>
    <p>Guaranteed economic stability: no inflation and crises, stable and same prices around the world, a single monetary unit, and a fixed minimal taxation or no tax.</p>
    <p>The security of Human and society from any kind of threats is ensured by the unified global service that deals with emergency situations.</p>
  </div>
  <div>
    <h3>4. Transparency and openness of information for all</h3>
    <p>Every Human has the right to receive reliable information about the movement and distribution of public funds. Each Human has access to information about the status of implementation of the society’s decisions.</p>
    <p>The mass media belong exclusively to the society and reflect information truthfully, openly, and honestly.</p>
  </div>
  <div>
    <h3>5. The creative ideology</h3>
    <p>Ideology should be aimed at popularizing the best human qualities and stopping everything that is directed against a Human. The main priority is the priority of humanity, high spiritual and moral aspirations of a Human, humanness, virtue, mutual respect and strengthening of friendship.</p>
    <p>Creating conditions for the development and education of a Human with a capital “H”, cultivating moral values in each person and society.</p>
    <p>Prohibition of propaganda of violence, condemnation and denunciation of any form of division, aggression, and anti-humane manifestations.</p>
  </div>
  <div>
    <h3>6. Development of Personality</h3>
    <p>Every person in the Creative society has the right to comprehensive development and personal fulfillment.</p>
    <p>Education should be free and equally accessible to all. Creating conditions and expanding opportunities for a Human to implement his or her creative abilities and talents.</p>
  </div>
  <div>
    <h3>7. Justice and equality</h3>
    <p>All natural resources belong to Humans and are fairly distributed among all people. Monopolization of resources and their irrational use is prohibited. These resources are fairly distributed among the citizens of the entire Earth.</p>
    <p>A Human is guaranteed employment if he or she so desires. Pay for an identical position, specialty, or profession should be the same all over the world.</p>
    <p>Everyone has the right to private property and income, however within the limits of the individual's capitalization amount set by the society.</p>
  </div>
  <div>
    <h3>8. Self-governing society</h3>
    <p>The concept of "power" in the Creative society is absent, since the responsibility for society as a whole, its development, living conditions and harmonious format, lies with each Human.</p>
    <p>Everyone has the right to participate in the management of the affairs of the Creative Society and in the adoption of laws aimed at improving Human life.</p>
    <p>The solution of socially important, socially significant, and economic issues that affect the quality of a Human’s life is submitted for public discussion and voting (referendum).</p>
  </div>
  <div>
    <h3>Note to the 8 Pillars</h3>
    <p>In the Creative Society, thanks to the introduction of a new model of economy and new technologies, there will be no need to use money. Therefore, certain provisions of the 8 Pillars of the Creative Society, which proceed from the existence of monetary relations, will be relevant only in the transition period to the Creative Society.</p>
  </div>
</details>

Globalists? Yes. Sounds unrealistic and unfeasible? But the more support there is, the faster it will come true. All that is asked of you is to read about it and tell others. Now that you know about this project, half of the work is done, and whether to support it or not is a personal matter for everyone, although I would really like to finally live in a world where there is peace and justice...

## 💸 Project support

Currently there is no need for financial support. I do have a full time job, and while it's not much, it's manageable.

> <details>
>   <summary>Literally me</summary>
>   <div align="center">
>     <img src="https://github.com/DigitalE4rth/AuroraDialogEnhancer/blob/readme-resources/honest_work_meme.jpg" />
>   </div>
> </details>

The best thing you can do with your money, if you don't know how to spend it, is to spread the word about Creative Society. This is more essential right now 💖.

Other than that, you can leave a `⭐ Star` for this project, and I would be very grateful for that).

<div align="center">
  <h3>☮️ PEACE</h3>
</div>
