#!/usr/bin/env bash
# PostToolUse (Edit|Write): syntax-check edited Python tool scripts.
# (C# has no fast out-of-editor check — Unity compiles on focus; tests run
# in batchmode with the editor closed. Deliberately no Stop hook here.)
input=$(cat)
file=$(printf '%s' "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | sed 's/\\\\/\\/g')
case "$file" in
  *.py)
    out=$(py -m py_compile "$file" 2>&1) || {
      echo "Python syntax error in $file:" >&2
      echo "$out" >&2
      exit 2
    }
    ;;
esac
exit 0
