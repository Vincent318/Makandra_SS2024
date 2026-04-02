# Datei: git-acp.ps1

# Änderungen hinzufügen
git add .

# Commit mit aktuellem Datum und Uhrzeit
$commitMessage = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
git commit -m "$commitMessage"

# Änderungen pushen
git push
