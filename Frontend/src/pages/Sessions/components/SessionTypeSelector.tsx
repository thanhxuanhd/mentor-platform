import type { SessionType } from "../../../types/enums/SessionType";

interface SessionTypeSelectorProps {
  sessionTypes: SessionType[];
  selectedSessionType: SessionType | null;
  onSessionTypeSelect: (type: SessionType) => void;
}

const formatSessionType = (type: SessionType): string => {
  switch (type) {
    case "OneOnOne":
      return "One on One";
    case "OnSite":
      return "On-Site";
    default:
      return type;
  }
};

export default function SessionTypeSelector({
  sessionTypes,
  selectedSessionType,
  onSessionTypeSelect
}: SessionTypeSelectorProps) {
  return (
    <div className="mt-8">
      <h3 className="text-lg font-medium mb-4 text-center">Session type</h3>
      <div className="grid grid-cols-3 gap-4">
        {sessionTypes.map((type) => (
          <div
            key={type}
            className={`
              cursor-pointer transition-all rounded-lg border-2 p-6
              ${selectedSessionType === type
                ? "border-orange-500 bg-slate-600"
                : "border-slate-600 bg-slate-700 hover:border-slate-500"
              }
            `}
            onClick={() => onSessionTypeSelect(type)}
          >
            <div className="text-center">
              <h4
                className={`font-medium ${selectedSessionType === type ? "text-white" : "text-gray-300"
                  }`}
              >
                {formatSessionType(type)}
              </h4>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}