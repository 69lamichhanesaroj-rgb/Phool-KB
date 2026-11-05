# CDS.infrastructure – Kvist & Bjælke ApS

**Group:** Phool | **Semester:** 3rd | **Platform:** Proxmox 

---

## 1. Company requirements

| Device | Quantity | Purpose |
|---|---:|---|
| Desktop computers | 15 | Administration and office work |
| Laptops | 10 | Employees working from different locations |
| Network printers | 3 | Shared printing |
| Servers | 3 | Internal systems and applications |
| IP phones | 12 | Telephony over the company network |
| Wireless access points | 4 | Employee and guest Wi-Fi |
| Surveillance cameras | 6 | IP-based video surveillance |
| Network equipment | As required | Switches, routers, firewall |
| Guest devices | Variable | Visitors' phones and computers |

About 60 devices today. The design leaves room for growth (more employees, more cameras, more servers).

---

## 2. IP addressing plan

### 2.1 Private range

We use the private range **10.0.0.0/8** (RFC 1918), because the assignment already fixes VLAN 570 and 571 as `10.5.70.0/24` and `10.5.71.0/24`. We follow the same pattern for the other networks: **VLAN 57x → subnet `10.5.7x.0`**. The VLAN ID can be read from the address and the other way round.

### 2.2 Networks

| VLAN | Name | Subnet | Mask | Gateway | Devices |
|---:|---|---|---|---|---|
| **570** | Clients | `10.5.70.0/24` | 255.255.255.0 | `10.5.70.5` | 15 desktops, 10 laptops, staff Wi-Fi |
| **571** | Servers | `10.5.71.0/24` | 255.255.255.0 | `10.5.71.5` | 3 servers (login platform VM) |
| 572 | Voice | `10.5.72.0/24` | 255.255.255.0 | `10.5.72.5` | 12 IP phones |
| 573 | Printers | `10.5.73.0/27` | 255.255.255.224 | `10.5.73.5` | 3 network printers |
| 574 | Cameras | `10.5.74.0/26` | 255.255.255.192 | `10.5.74.5` | 6 IP cameras (and a recorder) |
| 575 | Guest | `10.5.75.0/24` | 255.255.255.0 | `10.5.75.5` | Guest Wi-Fi devices |
| 576 | Management | `10.5.76.0/27` | 255.255.255.224 | `10.5.76.5` | Switches, access points, firewall admin |

**Gateway addresses.** The gateway of VLAN 571 is `10.5.71.5`. We found it by inspecting the network, the common `.1` convention does **not** apply in this environment. The firewall/router owns the gateway address in every VLAN, so we use the same `.5` pattern for the other VLANs. VLAN 570's gateway `10.5.70.5` is confirmed from the client's `ip route` output. VLANs 572–576 are a proposal and are not implemented.

### 2.3 Why we segment the network

| Reason | Explanation |
|---|---|
| **Security** | An infected guest phone or a weak IoT camera cannot reach the servers when they are in other networks. |
| **Control** | The firewall can only filter traffic *between* networks. With separate VLANs it can say "clients may reach the server only on TCP 8000–8100". |
| **Performance** | Broadcasts (e.g. ARP) stay inside one VLAN, so each network is a smaller broadcast domain. |
| **Management** | Rules, troubleshooting and documentation are simpler when each network has one purpose. |
| **Growth** | New devices can be added to one segment without disturbing the others. |

| Device group | Own network? | Justification |
|---|---|---|
| Servers | Yes (571) | Most valuable assets, strictest protection. |
| Employee PCs and laptops | Yes (570) | Trusted users, but they browse the web and are a common infection route. |
| IP phones | Yes (572) | Voice needs priority (QoS) and should be isolated from PC problems. |
| Printers | Yes (573) | Often old devices with weak security. |
| Cameras | Yes (574) | IoT devices with weak security; should not reach servers or the Internet freely. |
| Guests | Yes (575) | Untrusted: Internet only, no internal access. |
| Network equipment | Yes (576) | Only administrators should be able to configure it. |

### 2.4 Static IP addresses vs. DHCP

| Device | Method | Why |
|---|---|---|
| Servers | **Static** | Other systems must always find them at the same address. |
| Switches, access points, firewall | **Static** | Must always be reachable for management. |
| Printers, cameras | **Static or DHCP reservation** | Computers and the recorder need a fixed address to reach them. |
| Desktops, laptops | **DHCP** | Many devices; laptops move around. |
| IP phones | **DHCP** | Easy mass deployment. |
| Guest devices | **DHCP** | Unknown, changing devices. |

Each subnet is divided into zones so that an address shows whether a device is static or dynamic. Example for VLAN 570:

| Range | Use |
|---|---|
| `10.5.70.5` | Gateway |
| `10.5.70.2–.49` (except .5) | Static addresses and reservations |
| `10.5.70.50–.250` | DHCP pool |
| `10.5.70.251–.254` | Reserved |

In our lab, the **server VM uses a static address** (`10.5.71.74`) and the **client VM receives its address by DHCP**.

---

## 3. Subnetting

Usable hosts = 2^(host bits) − 2. The first address is the network address and the last is the broadcast address; neither can be assigned to a device.

| Network | Network address | Mask / CIDR | Host bits | Usable hosts | Range | Broadcast |
|---|---|---|---:|---:|---|---|
| Clients (570) | `10.5.70.0` | 255.255.255.0 (/24) | 8 | **254** | .1 – .254 | `10.5.70.255` |
| Servers (571) | `10.5.71.0` | 255.255.255.0 (/24) | 8 | **254** | .1 – .254 | `10.5.71.255` |
| Printers (573) | `10.5.73.0` | 255.255.255.224 (/27) | 5 | **30** | .1 – .30 | `10.5.73.31` |
| Cameras (574) | `10.5.74.0` | 255.255.255.192 (/26) | 6 | **62** | .1 – .62 | `10.5.74.63` |

**Why these sizes fit**

- **Clients /24 (254 hosts).** There are 25 computers today, plus phones and tablets on Wi-Fi. A /24 covers that about ten times over, so the company can grow without renumbering.
- **Servers /24 (254 hosts).** Only 3 servers today, but test servers, databases and backup systems accumulate. The extra space is deliberate. The size is also given by the assignment.
- **Printers /27 (30 hosts).** 3 printers plus a gateway use 4 addresses. A /28 (14 hosts) would be enough, but /27 leaves room for scanners and more printers without wasting much.
- **Cameras /26 (62 hosts).** 6 cameras, a gateway and a recorder use about 8 addresses. Cameras are often added when coverage is extended, so /26 gives about ten times the need.

Block size = 256 − last mask octet (/27: 256 − 224 = 32; /26: 256 − 192 = 64), so same-size subnets stack neatly: `10.5.73.0/27`, `10.5.73.32/27`, and so on. All subnets sit inside `10.5.0.0/16`, which leaves a lot of unused space for future VLANs (577, 578, …) such as a DMZ.

---

## 4. Network diagram

Proposed architecture for the whole company. Only the parts marked **implemented** were built in the virtual environment (VLAN 570 client, VLAN 571 server).



## 5. Server VM on VLAN 571

### 5.1 Virtual machine

| Parameter | Value |
|---|---|
| Hypervisor | Proxmox VE, node `pve02`, VM ID 104, name `Phool` |
| Operating system | **Ubuntu Server 26.04.1 LTS** (standard install, not minimized) |
| Network | Bridge / vnet **`VLAN571`**, model VirtIO, **VLAN Tag empty** |
| vCPU / RAM | 4096 MB 
| Disk | about 15 GB (19 % used after Docker and the application) |
| Hostname / user | `phool` / `phool` |
| SSH | OpenSSH server installed |


### 5.2 Ubuntu Server installation

Installed from the Ubuntu Server ISO with the text installer: English, standard "Ubuntu Server" (not minimized), entire disk, OpenSSH server enabled, no extra snaps. At the network step, `ens18` was set to a **manual IPv4 configuration**:

| Setting | Value |
|---|---|
| Subnet | `10.5.71.0/24` |
| Address | `10.5.71.74` |
| Gateway | `10.5.71.5` |
| DNS | `1.1.1.1`, `8.8.8.8` |



### 5.3 IP configuration

```text
$ ip a        ->  ens18: UP, inet 10.5.71.74/24, brd 10.5.71.255
$ ip route    ->  default via 10.5.71.5 dev ens18
                  10.5.71.0/24 dev ens18 proto kernel scope link src 10.5.71.74
$ ping 10.5.71.5     -> 4/4 replies, 0 % loss (about 0.3 ms)
$ ping google.com    -> 4/4 replies, 0 % loss (about 23 ms)
```


## 6. Docker installation and deployment

### 6.1 Docker installation

Docker Engine was installed on the server VM  and verified:

```bash
sudo apt update
sudo apt install -y curl
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

docker --version
sudo systemctl status docker
sudo docker run hello-world
```
### 6.2 Getting the application onto the server

```bash
sudo apt install -y git
git clone https://github.com/69lamichhanesaroj-rgb/Phool-KB.git
cd Phool-KB
```

### 6.3 Deployment with Docker Compose

The login platform consists of two parts, each with its own Dockerfile, started together by one compose file:

| Container | Source | Inside the container | Published on the server | Purpose |
|---|---|---|---|---|
| `phool-web` | `client/` (React, served by Bun) | 3000 | **8080** | Login page / user interface |
| `phool-api` | `server/API` (ASP.NET, .NET 10, SQLite) | 8080 | **8081** | REST API, authentication, database |

`docker-compose.yml` (repository root):

```yaml
services:
  api:
    build:
      context: .
      dockerfile: server/API/Dockerfile
    container_name: phool-api
    restart: unless-stopped
    ports:
      - "8081:8080"
    environment:
      - ConnectionStrings__Default=Data Source=/app/data/phool.db
      - Cors__ClientOrigin=http://10.5.71.74:8080
    volumes:
      - phool_data:/app/data

  web:
    build:
      context: ./client
    container_name: phool-web
    restart: unless-stopped
    ports:
      - "8080:3000"
    depends_on:
      - api

volumes:
  phool_data:
```

Start and verify:

```bash
sudo docker compose up -d --build
sudo docker ps
sudo docker compose ps
sudo ss -tulpn | grep -E "8080|8081"
curl -I http://localhost:8080
curl -i -X POST http://localhost:8081/api/auth/login -H "Content-Type: application/json" -d '{"username":"admin","password":"admin123"}'
```


![Login API test and listening ports](docs/images/server-api-login-ss.png)
*The API answers the login request with **200 OK** and a JWT token, and `ss -tulpn` shows ports 8080 and 8081 listening on all interfaces (`0.0.0.0`) through `docker-proxy`.*


## 7. Client VM on VLAN 570

The client VM is a second virtual machine used to test access to the login platform from a different network than the server.

| Parameter | Value |
|---|---|
| Operating system | Ubuntu Desktop (a browser is needed to use the login page) |
| Network | Proxmox bridge **`vmbr0`**, **VLAN Tag empty** (untagged). Setting tag 570 on `vmbr0` gave no working network |
| IP configuration | DHCP |
| Address received | **`10.5.70.99/24`** (inside `10.5.70.0/24`) |
| Default gateway | **`10.5.70.5`** (handed out by DHCP) |
| Interface / MAC | `ens18` / `bc:24:11:81:b4:01` |

The Proxmox environment offers only one SDN vnet (`VLAN571`, "Servers") and the normal bridge `vmbr0`. The client therefore uses `vmbr0` without a VLAN tag, which places it in the client network `10.5.70.0/24` (we tested `vmbr0` with tag 570 as well, but only the untagged setting received an address). A likely explanation is that the physical port behind `vmbr0` already carries VLAN 570 untagged, so the college's network delivers the client VLAN without any tag from the VM. The server VM uses the vnet `VLAN571`.


## 8. VLANs, routing and the firewall explained

**Why are VLAN 570 and VLAN 571 separate networks?**
They hold devices with different trust levels and purposes. Servers hold the company's applications and data and need stronger protection than employee machines that browse the web and may be infected. Separate networks also let the firewall control exactly what may pass between them, keep broadcasts inside each VLAN, and make rules and troubleshooting simpler.

**Why does communication between VLANs require routing?**
Each VLAN is its own subnet and broadcast domain. A switch only forwards frames inside one VLAN (layer 2), and a device only delivers directly to addresses in its own subnet. For any other subnet, it sends the packet to its default gateway. In our environment, the client `10.5.70.x` sends traffic for `10.5.71.74` to its gateway, and the router/firewall forwards it into VLAN 571 (layer 3).

**What is the role of the firewall?**
It sits between the networks (here it is also the router) and decides which traffic may pass. Rules match source, destination, protocol and port, and everything not explicitly allowed is denied. It is stateful, so replies to allowed connections return automatically. The school's firewall already allows VLAN 570 → VLAN 571 on TCP 8000–8100, so no firewall configuration was needed from us.

**Why allow only specific ports?**
It reduces the attack surface (only the intended service can be reached), follows the principle of least privilege, contains damage if a client is compromised (it cannot reach SSH, databases or file shares on the server), and makes traffic easier to monitor.

---

## 9. Final reflection

**How are security and infrastructure connected?**
A secure application is only as safe as the path leading to it. Argon2id password hashing and role checks protect the data inside the application, while VLAN segmentation, the firewall and port filtering protect the route to it. Allowing only TCP 8000–8100 between VLANs means that even a compromised client can reach nothing but the login platform. This is defense in depth.

**Technical challenges.**
Finding the real default gateway, fixing the Netplan YAML, understanding Proxmox SDN vnets, and making the frontend work from another machine (API address, CORS). See section 10.

**What we learned about deploying an application on a server.**
Containers make a deployment repeatable, but the network matters as much as the code: port mapping, the listen address (`0.0.0.0`, not `127.0.0.1`), CORS and a persistent volume must all be right. Testing bottom-up (VLAN → IP → gateway → port → application) and locally on the server first makes errors much easier to locate. The error type also tells something: *refused* means the host answers but nothing listens on the port, *timed out* usually means a firewall drops the packets, and *no route to host* means the router cannot find the destination.

**What we would do differently in a real company.**

| Area | Improvement |
|---|---|
| Encryption | HTTPS through a reverse proxy (nginx or Caddy); today passwords cross the network over plain HTTP. |
| Network | More VLANs (guest, voice, printers, cameras, management), a DMZ for anything Internet-facing, and a redundant firewall and uplink. |
| Secrets | Load the JWT signing key and other secrets from environment variables or a secrets manager instead of `appsettings.json`, and force a password change for the seeded demo users. |
| Access | SSH keys instead of passwords, a host firewall (UFW) as a second layer, and 802.1X for Wi-Fi. |
| Operations | Regular updates of the OS and container images, monitoring and logging, and backups of the Docker volume with a tested restore process. |
| Source control | Do not commit the SQLite database file (`phool.db`) to Git. |





##  Summary
We proposed a network with seven VLANs (clients, servers, voice, printers, cameras, guests, management). Each VLAN has its own subnet and gateway, and the two required networks are 10.5.70.0/24 (clients) and 10.5.71.0/24 (servers). In the virtual environment we built an Ubuntu Server VM on VLAN 571 (10.5.71.74) that runs the login platform in Docker on ports 8080 (web) and 8081 (API). A second VM on VLAN 570 (10.5.70.99) reached the platform through the school’s firewall, which allows only TCP 8000–8100. SSH and ping were blocked, as expected.

##  Reference

Docker Inc. (n.d.). Install Docker Engine on Ubuntu. Docker Docs. https://docs.docker.com/engine/install/ubuntu/

Canonical. (n.d.). Netplan documentation. https://netplan.io/

Anthropic. (2026). Claude (Sonnet 5.5) [Large language model]. https://claude.ai – used for explaining networking concepts and troubleshooting the deployment.
