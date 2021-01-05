# netping


*netping* allows a quick check / ping of groups of hosts. 

## Features
* Hosts can be stored in config files
* Hosts can be added to groups / removed from groups
* Pings can be run on
  * a list of host names (command line param)
  * a stored group of hostnames
  * all hostnames (using the *all* group)

Pings are executed asyncronously (allowing quick processing of a large number of hostnames).

If not running in verbose mode, only hosts without ping success are displayed (the failing hosts).

The default directory for the configuration file is 
`$HOME/.netping/config.json` or `%USERPROFILE%/.netping/config.json`.

## Usage:

```
 $ netping [-g group1 group2] [-a|-d|-D|-s] [-v] [-t] [-c file] [host1 ...]

  -a, --add             Add hostnames to list

  -d, --delete          Delete hostnames from list

  -s, --show            Show hostnames in list

  -D, --delete-group    Delete a group and it's host list

  -g, --groups          (Default: all) Hostname Groups. Show supports "*" for all groups.

  -v, --verbose         Show all ping results

  -c, --config          Configuration file

  -t, --timeout         (Default: 500) Timeout in ms

  --help                Display this help screen.

  --version             Display version information.
```


## Examples

Ping all known hosts. Only show the failing host names
```
$ netping
```

Ping all hosts showing responses or error messages
```
$ netping -v
```

Ping all hosts of the *db* group
```
$ netping -g db
```

Add some hosts to the *db* group
```
$ netping -g db -a dbhost01.my.domain dbhost02 mydbhost.domain.net
```

Delete a host from the *db* group
```
$ netping -g db -d mydbhost.domain.net
```

Show group *db*
```
$ netping -g db -s
```

Show all groups and their hosts
```
$ netping -g * -s
```

Delete the group *db*
```
$ netping -g db -D
```



