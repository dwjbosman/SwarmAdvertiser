#!/bin/bash
powershell.exe 'function Get-LocalIPAddress {param ([Parameter(Mandatory=$true)][string] $RemoteHost) $connection = Test-NetConnection -ComputerName $RemoteHost; return $connection.SourceAddress.IPAddress}; $ip=$(Get-LocalIPAddress '$1'); echo $ip;'
