# Ghost - a simple utility to get QBXML to & from QuickBooks

## What is this?
This is a spirit of transferrance, taking information from your app and delivering it into QuickBooks, then taking the message received from there & delivering back a response.

## Why use this?
Because it's simpler than writing something like this on your own.

## How do I use this?
Build it using VS 2010 or above (or your favorite C# compiler), and open QuickBooks.  It will use whatever file is open, and will (as of this writing) only accept TCP requests from `localhost` on port 3000.  Though it's asynchronous, so in theory you should be able to flood QB with enough information to make it cry.

## Why shouldn't I use this?
There is no error-checking, nothing to validate your QBXML and nothing to prevent you from breaking everything.  There really isn't much security besides only accepting info from `localhost` but that's flimsy at best.

## Is that all?
No, hopefully I will eventually implement an QBXML checker so some invalid requests get bounced back right away, without bothering QB.  Also an *actual* security model would be nice.

## AWESOME!!!  Can I help?
Sure!  Fork it, make your changes in a new branch, then make a pull request.
