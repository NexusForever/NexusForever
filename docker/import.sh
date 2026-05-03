#!/usr/bin/env bash
# Imports world SQL files into nexus_forever_world after EF migrations are done.
# Runs as a separate service so it executes AFTER migrate completes.
#
# Variables (passed via docker-compose environment):
#   DB_HOST, DB_PORT, DB_USER, DB_PASS, NO_IMPORT (optional)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WORLD_DB_DIR="$SCRIPT_DIR/world-database"
NO_IMPORT="${NO_IMPORT:-false}"

# ---------------------------------------------------------------------------
# Import world SQL
# ---------------------------------------------------------------------------
if [[ "$NO_IMPORT" == true ]]; then
  echo ""
  echo "SKIP   world SQL import (--no-import)"
else
  mapfile -t sql_files < <(find "$WORLD_DB_DIR" -type f -name '*.sql' ! -path '*/.git/*' | LC_ALL=C sort)
  if [[ ${#sql_files[@]} -eq 0 ]]; then
    echo ""
    echo "WARNING: no .sql files found in $WORLD_DB_DIR — skipping import."
  else
    echo ""
    echo "Importing ${#sql_files[@]} SQL file(s) into nexus_forever_world..."
    import_errors=0
    for f in "${sql_files[@]}"; do
      echo "  ${f#"$SCRIPT_DIR/"}"
      db_output=$(mariadb --force -h"${DB_HOST}" -P"${DB_PORT}" -u"${DB_USER}" -p"${DB_PASS}" nexus_forever_world < "$f" 2>&1) || true
      filtered=$(echo "$db_output" | grep -v "^$") || true
      if [[ -n "$filtered" ]]; then
        echo "  WARNING: errors in ${f#"$SCRIPT_DIR/"} (continuing)" >&2
        echo "$filtered" >&2
        (( import_errors++ )) || true
      fi
    done
    if [[ $import_errors -gt 0 ]]; then
      echo "World SQL import finished with $import_errors file(s) reporting errors (see above)."
    else
      echo "World SQL import finished."
    fi
  fi
fi
