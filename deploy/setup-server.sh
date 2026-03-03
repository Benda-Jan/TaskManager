#!/bin/bash
# Run once on a fresh Hetzner (Debian/Ubuntu) VPS as root.
set -euo pipefail

# 1. System update
apt-get update && apt-get upgrade -y

# 2. Install Docker
curl -fsSL https://get.docker.com | sh
systemctl enable docker

# 3. Install Docker Compose plugin (already bundled with modern Docker)
docker compose version

# 4. Harden SSH (disable password auth - assumes you added your key during VPS creation)
sed -i 's/^#\?PasswordAuthentication.*/PasswordAuthentication no/' /etc/ssh/sshd_config
systemctl reload sshd

# 5. Basic firewall
apt-get install -y ufw
ufw default deny incoming
ufw default allow outgoing
ufw allow ssh
ufw allow http
ufw allow https
ufw --force enable

echo ""
echo "Server ready. Next steps:"
echo "  1. Clone the repo:  git clone https://github.com/Benda-Jan/TaskManager.git"
echo "  2. cd TaskManager"
echo "  3. cp .env.prod.example .env.prod   # then fill in real values"
echo "  4. docker compose -f docker-compose.prod.yml --env-file .env.prod up -d --build"
echo "  5. Run migrations:  bash deploy/migrate.sh"
echo "  6. Configure Keycloak realm (see deploy/keycloak-setup.md)"
