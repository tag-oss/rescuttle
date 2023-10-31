# Gets the .NET Runtime for the given Architecture
# Needed because single binary builds in .NET 6 require an explicitly passed Runtime Identifier (RID)
# and we need to detect this for multi-arch builds in Docker
set +e 
if [ -n "$TARGET" ]; then 
  echo $TARGET
else
    
  case $TARGETPLATFORM in
  
    *aarch64*)
      echo "linux-arm64"
      ;;
    *arm64*)
      echo "linux-arm64"
      ;;
    *x86_64*)
      echo "linux-x64"
      ;;
    *amd64*)
      echo "linux-x64"
      ;;
    *)
      echo "unknown!!!!"
      exit 255
  esac

fi