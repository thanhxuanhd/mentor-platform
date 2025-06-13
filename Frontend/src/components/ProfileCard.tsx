import { Avatar } from "antd"
import type { UserContext, UserDetail } from "../types/UserTypes"
import DefaultAvatar from "../assets/images/default-account.svg"

interface UserProfileDropdownProps {
  userDetail?: UserDetail
  user: UserContext
}
export default function UserProfileDropdown({
  userDetail,
  user,
}: UserProfileDropdownProps) {

  return (
    <div className="flex items-center pr-2 gap-4 px-3 rounded-lg transition-all disabled duration-200 group">
      <div className="relative">
        <Avatar
          src={userDetail?.profilePhotoUrl || DefaultAvatar}
          size={32}
          className="ring-1 ring-slate-400/20 group-hover:ring-blue-400/40 transition-all duration-200"
        />
        <div
          className="absolute bottom-2.5 -right-1 w-3 h-3 rounded-full border-2 border-slate-700"
          style={{ backgroundColor: "#52c41a" }}
        />
      </div>
      <div className="hidden md:block text-left min-w-0">
        <div className="text-white text-lg font-medium">{userDetail?.fullName}</div>
        <div className="text-slate-400 text-xs truncate max-w-[120px] capitalize">{user.role}</div>
      </div>
    </div>
  )
}