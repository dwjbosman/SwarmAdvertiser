$swarm_manager_ip = $args[0]

netsh advfirewall firewall add rule name="node_communication_tcp" dir=in action=allow protocol=TCP localport=7946
netsh advfirewall firewall add rule name="node_communication_udp" dir=in action=allow protocol=UDP localport=7946
netsh advfirewall firewall add rule name="overlay_network" dir=in action=allow protocol=UDP localport=4789
netsh advfirewall firewall add rule name="swarm_dns_tcp" dir=in action=allow protocol=TCP localport=53
netsh advfirewall firewall add rule name="swarm_dns_udp" dir=in action=allow protocol=UDP localport=53
netsh advfirewall firewall add rule name="cluster_management" dir=in action=allow protocol=UDP localport=2377
netsh advfirewall firewall add rule name="cluster_management" dir=in action=allow protocol=TCP localport=2377

function Get-LocalIPAddress {
    param (
        [Parameter(Mandatory=$true)]
        [string] $RemoteHost
    )

    $connection = Test-NetConnection -RemoteAddress $RemoteHost
    return $connection.LocalAddress.IPAddressToString
}

$localip = Get-LocalIPAddress -RemoteHost $swarm_manager_ip
$wsl_ip = ((wsl hostname -I) -split " ")[0]

netsh interface portproxy add v4tov4 listenaddress=0.0.0.0 listenport=7946 connectaddress=$wslip connectport=7946
netsh interface portproxy add v4tov4 listenaddress=0.0.0.0 listenport=53 connectaddress=$wslip connectport=53
netsh interface portproxy add v4tov4 listenaddress=0.0.0.0 listenport=2377 connectaddress=$wslip connectport=2377

dotnet run $local_ip $wsl_ip 7946 4789 2377 53
