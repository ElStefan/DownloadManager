# DownloadManager


##What is that?
This version targets **.NET 8** and uses **gRPC** with Kestrel for hosting.

 - The *DownloadManager* is a gRPC service that manages urls of applications on a _mysql database_.
 - It's supposed to be running on a server in the local network, to give clients the requested application in a minimum of time.
 - A Windows Forms client written for **.NET 8** consumes the gRPC API and allows managing and downloading the apps.

##Why?

It's annoying to search for all my favourite applications every time i reinstall or fix a computer for me or my friends.
Having them on a stick is not a solution for me, because they get outdated in no time.
Now it's possible to get the newest version without all that searching, downloading and looking for files/folders.

##How?

The service stores the binaries until you want them, and the client can load and install it - **even without asking for a download folder or leaving lost setup files on your computer**.
You can select all applications you want, and tell the client to load and install one after another.

 _No more searching on the internet, just click through all the setup processes in a couple minutes!_

##Features

 - Quartz jobs are daily checking for link availability and looking on wikipedia for the newest versions.
 - **The service is currently only replacing version infos, not updating the url or file itself.**

##Feedback

 - Suggestions for improvement are always welcome!
