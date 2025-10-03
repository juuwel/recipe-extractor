import hashlib
import hmac
import os



def verify_webhook_token(token: str | None) -> bool:
    """Verify salted HMAC webhook token"""
    if not token:
        return False

    secret_key = os.getenv("WEBHOOK_SECRET_KEY")
    salt = os.getenv("WEBHOOK_SALT")

    if not secret_key or not salt:
        raise ValueError("Missing secret key or salt")

    # Create salted message
    salted_message = f"notion-webhook:{salt}"

    # Generate expected token
    expected_token = hmac.new(
        secret_key.encode(), salted_message.encode(), hashlib.sha512
    ).hexdigest()

    # Use constant-time comparison to prevent timing attacks
    return hmac.compare_digest(token, expected_token)
