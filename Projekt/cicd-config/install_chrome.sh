apt update -q -y
echo "Start installing Chrome..."
apt --yes install -q libnss3 xvfb libxi6 libgconf-2-4 unzip gnupg
echo "Preparations installed!"
wget -q https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
echo "Installing..."
apt install -q ./google-chrome-stable_current_amd64.deb -y
echo "installataion done!"