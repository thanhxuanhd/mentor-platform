import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";

export default function Test() {
  const navigate = useNavigate();
  const { state } = useLocation();

  useEffect(() => {
    if (!state) {
      navigate('/login')
      return;
    }
  }, [])

  return (
    <div>{state ? `${state.userId} + ${state.token} + ${state.userStatus}` : "no"}</div>
  )
}