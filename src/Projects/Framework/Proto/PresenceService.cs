// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: bgs/low/pb/client/presence_service.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;
namespace Bgs.Protocol.Presence.V1
{

    /// <summary>Holder for reflection information generated from bgs/low/pb/client/presence_service.proto</summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class PresenceServiceReflection {

    #region Descriptor
    /// <summary>File descriptor for bgs/low/pb/client/presence_service.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static PresenceServiceReflection() {
      byte[] descriptorData = System.Convert.FromBase64String(
          string.Concat(
            "CihiZ3MvbG93L3BiL2NsaWVudC9wcmVzZW5jZV9zZXJ2aWNlLnByb3RvEhhi",
            "Z3MucHJvdG9jb2wucHJlc2VuY2UudjEaJGJncy9sb3cvcGIvY2xpZW50L2Vu",
            "dGl0eV90eXBlcy5wcm90bxomYmdzL2xvdy9wYi9jbGllbnQvcHJlc2VuY2Vf",
            "dHlwZXMucHJvdG8aIWJncy9sb3cvcGIvY2xpZW50L3JwY190eXBlcy5wcm90",
            "byKkAQoQU3Vic2NyaWJlUmVxdWVzdBIoCghhZ2VudF9pZBgBIAEoCzIWLmJn",
            "cy5wcm90b2NvbC5FbnRpdHlJZBIpCgllbnRpdHlfaWQYAiABKAsyFi5iZ3Mu",
            "cHJvdG9jb2wuRW50aXR5SWQSEQoJb2JqZWN0X2lkGAMgASgEEg8KB3Byb2dy",
            "YW0YBCADKAcSFwoLZmxhZ19wdWJsaWMYBSABKAhCAhgBIkkKHFN1YnNjcmli",
            "ZU5vdGlmaWNhdGlvblJlcXVlc3QSKQoJZW50aXR5X2lkGAEgASgLMhYuYmdz",
            "LnByb3RvY29sLkVudGl0eUlkInwKElVuc3Vic2NyaWJlUmVxdWVzdBIoCghh",
            "Z2VudF9pZBgBIAEoCzIWLmJncy5wcm90b2NvbC5FbnRpdHlJZBIpCgllbnRp",
            "dHlfaWQYAiABKAsyFi5iZ3MucHJvdG9jb2wuRW50aXR5SWQSEQoJb2JqZWN0",
            "X2lkGAMgASgEIroBCg1VcGRhdGVSZXF1ZXN0EikKCWVudGl0eV9pZBgBIAEo",
            "CzIWLmJncy5wcm90b2NvbC5FbnRpdHlJZBJBCg9maWVsZF9vcGVyYXRpb24Y",
            "AiADKAsyKC5iZ3MucHJvdG9jb2wucHJlc2VuY2UudjEuRmllbGRPcGVyYXRp",
            "b24SEQoJbm9fY3JlYXRlGAMgASgIEigKCGFnZW50X2lkGAQgASgLMhYuYmdz",
            "LnByb3RvY29sLkVudGl0eUlkIpQBCgxRdWVyeVJlcXVlc3QSKQoJZW50aXR5",
            "X2lkGAEgASgLMhYuYmdzLnByb3RvY29sLkVudGl0eUlkEi8KA2tleRgCIAMo",
            "CzIiLmJncy5wcm90b2NvbC5wcmVzZW5jZS52MS5GaWVsZEtleRIoCghhZ2Vu",
            "dF9pZBgDIAEoCzIWLmJncy5wcm90b2NvbC5FbnRpdHlJZCI/Cg1RdWVyeVJl",
            "c3BvbnNlEi4KBWZpZWxkGAIgAygLMh8uYmdzLnByb3RvY29sLnByZXNlbmNl",
            "LnYxLkZpZWxkIlgKEE93bmVyc2hpcFJlcXVlc3QSKQoJZW50aXR5X2lkGAEg",
            "ASgLMhYuYmdzLnByb3RvY29sLkVudGl0eUlkEhkKEXJlbGVhc2Vfb3duZXJz",
            "aGlwGAIgASgIMowECg9QcmVzZW5jZVNlcnZpY2USTQoJU3Vic2NyaWJlEiou",
            "YmdzLnByb3RvY29sLnByZXNlbmNlLnYxLlN1YnNjcmliZVJlcXVlc3QaFC5i",
            "Z3MucHJvdG9jb2wuTm9EYXRhElEKC1Vuc3Vic2NyaWJlEiwuYmdzLnByb3Rv",
            "Y29sLnByZXNlbmNlLnYxLlVuc3Vic2NyaWJlUmVxdWVzdBoULmJncy5wcm90",
            "b2NvbC5Ob0RhdGESRwoGVXBkYXRlEicuYmdzLnByb3RvY29sLnByZXNlbmNl",
            "LnYxLlVwZGF0ZVJlcXVlc3QaFC5iZ3MucHJvdG9jb2wuTm9EYXRhElgKBVF1",
            "ZXJ5EiYuYmdzLnByb3RvY29sLnByZXNlbmNlLnYxLlF1ZXJ5UmVxdWVzdBon",
            "LmJncy5wcm90b2NvbC5wcmVzZW5jZS52MS5RdWVyeVJlc3BvbnNlEk0KCU93",
            "bmVyc2hpcBIqLmJncy5wcm90b2NvbC5wcmVzZW5jZS52MS5Pd25lcnNoaXBS",
            "ZXF1ZXN0GhQuYmdzLnByb3RvY29sLk5vRGF0YRJlChVTdWJzY3JpYmVOb3Rp",
            "ZmljYXRpb24SNi5iZ3MucHJvdG9jb2wucHJlc2VuY2UudjEuU3Vic2NyaWJl",
            "Tm90aWZpY2F0aW9uUmVxdWVzdBoULmJncy5wcm90b2NvbC5Ob0RhdGFCBUgC",
            "gAEAYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { Bgs.Protocol.EntityTypesReflection.Descriptor, Bgs.Protocol.Presence.V1.PresenceTypesReflection.Descriptor, Bgs.Protocol.RpcTypesReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.SubscribeRequest), Bgs.Protocol.Presence.V1.SubscribeRequest.Parser, new[]{ "AgentId", "EntityId", "ObjectId", "Program", "FlagPublic" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.SubscribeNotificationRequest), Bgs.Protocol.Presence.V1.SubscribeNotificationRequest.Parser, new[]{ "EntityId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.UnsubscribeRequest), Bgs.Protocol.Presence.V1.UnsubscribeRequest.Parser, new[]{ "AgentId", "EntityId", "ObjectId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.UpdateRequest), Bgs.Protocol.Presence.V1.UpdateRequest.Parser, new[]{ "EntityId", "FieldOperation", "NoCreate", "AgentId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.QueryRequest), Bgs.Protocol.Presence.V1.QueryRequest.Parser, new[]{ "EntityId", "Key", "AgentId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.QueryResponse), Bgs.Protocol.Presence.V1.QueryResponse.Parser, new[]{ "Field" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(Bgs.Protocol.Presence.V1.OwnershipRequest), Bgs.Protocol.Presence.V1.OwnershipRequest.Parser, new[]{ "EntityId", "ReleaseOwnership" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class SubscribeRequest : pb::IMessage<SubscribeRequest> {
    private static readonly pb::MessageParser<SubscribeRequest> _parser = new pb::MessageParser<SubscribeRequest>(() => new SubscribeRequest());
    public static pb::MessageParser<SubscribeRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[0]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public SubscribeRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public SubscribeRequest(SubscribeRequest other) : this() {
      AgentId = other.agentId_ != null ? other.AgentId.Clone() : null;
      EntityId = other.entityId_ != null ? other.EntityId.Clone() : null;
      objectId_ = other.objectId_;
      program_ = other.program_.Clone();
      flagPublic_ = other.flagPublic_;
    }

    public SubscribeRequest Clone() {
      return new SubscribeRequest(this);
    }

    /// <summary>Field number for the "agent_id" field.</summary>
    public const int AgentIdFieldNumber = 1;
    private Bgs.Protocol.EntityId agentId_;
    public Bgs.Protocol.EntityId AgentId {
      get { return agentId_; }
      set {
        agentId_ = value;
      }
    }

    /// <summary>Field number for the "entity_id" field.</summary>
    public const int EntityIdFieldNumber = 2;
    private Bgs.Protocol.EntityId entityId_;
    public Bgs.Protocol.EntityId EntityId {
      get { return entityId_; }
      set {
        entityId_ = value;
      }
    }

    /// <summary>Field number for the "object_id" field.</summary>
    public const int ObjectIdFieldNumber = 3;
    private ulong objectId_;
    public ulong ObjectId {
      get { return objectId_; }
      set {
        objectId_ = value;
      }
    }

    /// <summary>Field number for the "program" field.</summary>
    public const int ProgramFieldNumber = 4;
    private static readonly pb::FieldCodec<uint> _repeated_program_codec
        = pb::FieldCodec.ForFixed32(34);
    private readonly pbc::RepeatedField<uint> program_ = new pbc::RepeatedField<uint>();
    public pbc::RepeatedField<uint> Program {
      get { return program_; }
    }

    /// <summary>Field number for the "flag_public" field.</summary>
    public const int FlagPublicFieldNumber = 5;
    private bool flagPublic_;
    [System.ObsoleteAttribute()]
    public bool FlagPublic {
      get { return flagPublic_; }
      set {
        flagPublic_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as SubscribeRequest);
    }

    public bool Equals(SubscribeRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(AgentId, other.AgentId)) return false;
      if (!object.Equals(EntityId, other.EntityId)) return false;
      if (ObjectId != other.ObjectId) return false;
      if(!program_.Equals(other.program_)) return false;
      if (FlagPublic != other.FlagPublic) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (agentId_ != null) hash ^= AgentId.GetHashCode();
      if (entityId_ != null) hash ^= EntityId.GetHashCode();
      if (ObjectId != 0UL) hash ^= ObjectId.GetHashCode();
      hash ^= program_.GetHashCode();
      if (FlagPublic != false) hash ^= FlagPublic.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (agentId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(AgentId);
      }
      if (entityId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(EntityId);
      }
      if (ObjectId != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(ObjectId);
      }
      program_.WriteTo(output, _repeated_program_codec);
      if (FlagPublic != false) {
        output.WriteRawTag(40);
        output.WriteBool(FlagPublic);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (agentId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AgentId);
      }
      if (entityId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EntityId);
      }
      if (ObjectId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ObjectId);
      }
      size += program_.CalculateSize(_repeated_program_codec);
      if (FlagPublic != false) {
        size += 1 + 1;
      }
      return size;
    }

    public void MergeFrom(SubscribeRequest other) {
      if (other == null) {
        return;
      }
      if (other.agentId_ != null) {
        if (agentId_ == null) {
          agentId_ = new Bgs.Protocol.EntityId();
        }
        AgentId.MergeFrom(other.AgentId);
      }
      if (other.entityId_ != null) {
        if (entityId_ == null) {
          entityId_ = new Bgs.Protocol.EntityId();
        }
        EntityId.MergeFrom(other.EntityId);
      }
      if (other.ObjectId != 0UL) {
        ObjectId = other.ObjectId;
      }
      program_.Add(other.program_);
      if (other.FlagPublic != false) {
        FlagPublic = other.FlagPublic;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (agentId_ == null) {
              agentId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(agentId_);
            break;
          }
          case 18: {
            if (entityId_ == null) {
              entityId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(entityId_);
            break;
          }
          case 24: {
            ObjectId = input.ReadUInt64();
            break;
          }
          case 34:
          case 37: {
            program_.AddEntriesFrom(input, _repeated_program_codec);
            break;
          }
          case 40: {
            FlagPublic = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class SubscribeNotificationRequest : pb::IMessage<SubscribeNotificationRequest> {
    private static readonly pb::MessageParser<SubscribeNotificationRequest> _parser = new pb::MessageParser<SubscribeNotificationRequest>(() => new SubscribeNotificationRequest());
    public static pb::MessageParser<SubscribeNotificationRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[1]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public SubscribeNotificationRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public SubscribeNotificationRequest(SubscribeNotificationRequest other) : this() {
      EntityId = other.entityId_ != null ? other.EntityId.Clone() : null;
    }

    public SubscribeNotificationRequest Clone() {
      return new SubscribeNotificationRequest(this);
    }

    /// <summary>Field number for the "entity_id" field.</summary>
    public const int EntityIdFieldNumber = 1;
    private Bgs.Protocol.EntityId entityId_;
    public Bgs.Protocol.EntityId EntityId {
      get { return entityId_; }
      set {
        entityId_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as SubscribeNotificationRequest);
    }

    public bool Equals(SubscribeNotificationRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(EntityId, other.EntityId)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (entityId_ != null) hash ^= EntityId.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (entityId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(EntityId);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (entityId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EntityId);
      }
      return size;
    }

    public void MergeFrom(SubscribeNotificationRequest other) {
      if (other == null) {
        return;
      }
      if (other.entityId_ != null) {
        if (entityId_ == null) {
          entityId_ = new Bgs.Protocol.EntityId();
        }
        EntityId.MergeFrom(other.EntityId);
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (entityId_ == null) {
              entityId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(entityId_);
            break;
          }
        }
      }
    }

  }

  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class UnsubscribeRequest : pb::IMessage<UnsubscribeRequest> {
    private static readonly pb::MessageParser<UnsubscribeRequest> _parser = new pb::MessageParser<UnsubscribeRequest>(() => new UnsubscribeRequest());
    public static pb::MessageParser<UnsubscribeRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[2]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public UnsubscribeRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public UnsubscribeRequest(UnsubscribeRequest other) : this() {
      AgentId = other.agentId_ != null ? other.AgentId.Clone() : null;
      EntityId = other.entityId_ != null ? other.EntityId.Clone() : null;
      objectId_ = other.objectId_;
    }

    public UnsubscribeRequest Clone() {
      return new UnsubscribeRequest(this);
    }

    /// <summary>Field number for the "agent_id" field.</summary>
    public const int AgentIdFieldNumber = 1;
    private Bgs.Protocol.EntityId agentId_;
    public Bgs.Protocol.EntityId AgentId {
      get { return agentId_; }
      set {
        agentId_ = value;
      }
    }

    /// <summary>Field number for the "entity_id" field.</summary>
    public const int EntityIdFieldNumber = 2;
    private Bgs.Protocol.EntityId entityId_;
    public Bgs.Protocol.EntityId EntityId {
      get { return entityId_; }
      set {
        entityId_ = value;
      }
    }

    /// <summary>Field number for the "object_id" field.</summary>
    public const int ObjectIdFieldNumber = 3;
    private ulong objectId_;
    public ulong ObjectId {
      get { return objectId_; }
      set {
        objectId_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as UnsubscribeRequest);
    }

    public bool Equals(UnsubscribeRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(AgentId, other.AgentId)) return false;
      if (!object.Equals(EntityId, other.EntityId)) return false;
      if (ObjectId != other.ObjectId) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (agentId_ != null) hash ^= AgentId.GetHashCode();
      if (entityId_ != null) hash ^= EntityId.GetHashCode();
      if (ObjectId != 0UL) hash ^= ObjectId.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (agentId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(AgentId);
      }
      if (entityId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(EntityId);
      }
      if (ObjectId != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(ObjectId);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (agentId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AgentId);
      }
      if (entityId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EntityId);
      }
      if (ObjectId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ObjectId);
      }
      return size;
    }

    public void MergeFrom(UnsubscribeRequest other) {
      if (other == null) {
        return;
      }
      if (other.agentId_ != null) {
        if (agentId_ == null) {
          agentId_ = new Bgs.Protocol.EntityId();
        }
        AgentId.MergeFrom(other.AgentId);
      }
      if (other.entityId_ != null) {
        if (entityId_ == null) {
          entityId_ = new Bgs.Protocol.EntityId();
        }
        EntityId.MergeFrom(other.EntityId);
      }
      if (other.ObjectId != 0UL) {
        ObjectId = other.ObjectId;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (agentId_ == null) {
              agentId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(agentId_);
            break;
          }
          case 18: {
            if (entityId_ == null) {
              entityId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(entityId_);
            break;
          }
          case 24: {
            ObjectId = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class UpdateRequest : pb::IMessage<UpdateRequest> {
    private static readonly pb::MessageParser<UpdateRequest> _parser = new pb::MessageParser<UpdateRequest>(() => new UpdateRequest());
    public static pb::MessageParser<UpdateRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[3]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public UpdateRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public UpdateRequest(UpdateRequest other) : this() {
      EntityId = other.entityId_ != null ? other.EntityId.Clone() : null;
      fieldOperation_ = other.fieldOperation_.Clone();
      noCreate_ = other.noCreate_;
      AgentId = other.agentId_ != null ? other.AgentId.Clone() : null;
    }

    public UpdateRequest Clone() {
      return new UpdateRequest(this);
    }

    /// <summary>Field number for the "entity_id" field.</summary>
    public const int EntityIdFieldNumber = 1;
    private Bgs.Protocol.EntityId entityId_;
    public Bgs.Protocol.EntityId EntityId {
      get { return entityId_; }
      set {
        entityId_ = value;
      }
    }

    /// <summary>Field number for the "field_operation" field.</summary>
    public const int FieldOperationFieldNumber = 2;
    private static readonly pb::FieldCodec<Bgs.Protocol.Presence.V1.FieldOperation> _repeated_fieldOperation_codec
        = pb::FieldCodec.ForMessage(18, Bgs.Protocol.Presence.V1.FieldOperation.Parser);
    private readonly pbc::RepeatedField<Bgs.Protocol.Presence.V1.FieldOperation> fieldOperation_ = new pbc::RepeatedField<Bgs.Protocol.Presence.V1.FieldOperation>();
    public pbc::RepeatedField<Bgs.Protocol.Presence.V1.FieldOperation> FieldOperation {
      get { return fieldOperation_; }
    }

    /// <summary>Field number for the "no_create" field.</summary>
    public const int NoCreateFieldNumber = 3;
    private bool noCreate_;
    public bool NoCreate {
      get { return noCreate_; }
      set {
        noCreate_ = value;
      }
    }

    /// <summary>Field number for the "agent_id" field.</summary>
    public const int AgentIdFieldNumber = 4;
    private Bgs.Protocol.EntityId agentId_;
    public Bgs.Protocol.EntityId AgentId {
      get { return agentId_; }
      set {
        agentId_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as UpdateRequest);
    }

    public bool Equals(UpdateRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(EntityId, other.EntityId)) return false;
      if(!fieldOperation_.Equals(other.fieldOperation_)) return false;
      if (NoCreate != other.NoCreate) return false;
      if (!object.Equals(AgentId, other.AgentId)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (entityId_ != null) hash ^= EntityId.GetHashCode();
      hash ^= fieldOperation_.GetHashCode();
      if (NoCreate != false) hash ^= NoCreate.GetHashCode();
      if (agentId_ != null) hash ^= AgentId.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (entityId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(EntityId);
      }
      fieldOperation_.WriteTo(output, _repeated_fieldOperation_codec);
      if (NoCreate != false) {
        output.WriteRawTag(24);
        output.WriteBool(NoCreate);
      }
      if (agentId_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(AgentId);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (entityId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EntityId);
      }
      size += fieldOperation_.CalculateSize(_repeated_fieldOperation_codec);
      if (NoCreate != false) {
        size += 1 + 1;
      }
      if (agentId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AgentId);
      }
      return size;
    }

    public void MergeFrom(UpdateRequest other) {
      if (other == null) {
        return;
      }
      if (other.entityId_ != null) {
        if (entityId_ == null) {
          entityId_ = new Bgs.Protocol.EntityId();
        }
        EntityId.MergeFrom(other.EntityId);
      }
      fieldOperation_.Add(other.fieldOperation_);
      if (other.NoCreate != false) {
        NoCreate = other.NoCreate;
      }
      if (other.agentId_ != null) {
        if (agentId_ == null) {
          agentId_ = new Bgs.Protocol.EntityId();
        }
        AgentId.MergeFrom(other.AgentId);
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (entityId_ == null) {
              entityId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(entityId_);
            break;
          }
          case 18: {
            fieldOperation_.AddEntriesFrom(input, _repeated_fieldOperation_codec);
            break;
          }
          case 24: {
            NoCreate = input.ReadBool();
            break;
          }
          case 34: {
            if (agentId_ == null) {
              agentId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(agentId_);
            break;
          }
        }
      }
    }

  }

  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class QueryRequest : pb::IMessage<QueryRequest> {
    private static readonly pb::MessageParser<QueryRequest> _parser = new pb::MessageParser<QueryRequest>(() => new QueryRequest());
    public static pb::MessageParser<QueryRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[4]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public QueryRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public QueryRequest(QueryRequest other) : this() {
      EntityId = other.entityId_ != null ? other.EntityId.Clone() : null;
      key_ = other.key_.Clone();
      AgentId = other.agentId_ != null ? other.AgentId.Clone() : null;
    }

    public QueryRequest Clone() {
      return new QueryRequest(this);
    }

    /// <summary>Field number for the "entity_id" field.</summary>
    public const int EntityIdFieldNumber = 1;
    private Bgs.Protocol.EntityId entityId_;
    public Bgs.Protocol.EntityId EntityId {
      get { return entityId_; }
      set {
        entityId_ = value;
      }
    }

    /// <summary>Field number for the "key" field.</summary>
    public const int KeyFieldNumber = 2;
    private static readonly pb::FieldCodec<Bgs.Protocol.Presence.V1.FieldKey> _repeated_key_codec
        = pb::FieldCodec.ForMessage(18, Bgs.Protocol.Presence.V1.FieldKey.Parser);
    private readonly pbc::RepeatedField<Bgs.Protocol.Presence.V1.FieldKey> key_ = new pbc::RepeatedField<Bgs.Protocol.Presence.V1.FieldKey>();
    public pbc::RepeatedField<Bgs.Protocol.Presence.V1.FieldKey> Key {
      get { return key_; }
    }

    /// <summary>Field number for the "agent_id" field.</summary>
    public const int AgentIdFieldNumber = 3;
    private Bgs.Protocol.EntityId agentId_;
    public Bgs.Protocol.EntityId AgentId {
      get { return agentId_; }
      set {
        agentId_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as QueryRequest);
    }

    public bool Equals(QueryRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(EntityId, other.EntityId)) return false;
      if(!key_.Equals(other.key_)) return false;
      if (!object.Equals(AgentId, other.AgentId)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (entityId_ != null) hash ^= EntityId.GetHashCode();
      hash ^= key_.GetHashCode();
      if (agentId_ != null) hash ^= AgentId.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (entityId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(EntityId);
      }
      key_.WriteTo(output, _repeated_key_codec);
      if (agentId_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(AgentId);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (entityId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EntityId);
      }
      size += key_.CalculateSize(_repeated_key_codec);
      if (agentId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AgentId);
      }
      return size;
    }

    public void MergeFrom(QueryRequest other) {
      if (other == null) {
        return;
      }
      if (other.entityId_ != null) {
        if (entityId_ == null) {
          entityId_ = new Bgs.Protocol.EntityId();
        }
        EntityId.MergeFrom(other.EntityId);
      }
      key_.Add(other.key_);
      if (other.agentId_ != null) {
        if (agentId_ == null) {
          agentId_ = new Bgs.Protocol.EntityId();
        }
        AgentId.MergeFrom(other.AgentId);
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (entityId_ == null) {
              entityId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(entityId_);
            break;
          }
          case 18: {
            key_.AddEntriesFrom(input, _repeated_key_codec);
            break;
          }
          case 26: {
            if (agentId_ == null) {
              agentId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(agentId_);
            break;
          }
        }
      }
    }

  }

  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class QueryResponse : pb::IMessage<QueryResponse> {
    private static readonly pb::MessageParser<QueryResponse> _parser = new pb::MessageParser<QueryResponse>(() => new QueryResponse());
    public static pb::MessageParser<QueryResponse> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[5]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public QueryResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    public QueryResponse(QueryResponse other) : this() {
      field_ = other.field_.Clone();
    }

    public QueryResponse Clone() {
      return new QueryResponse(this);
    }

    /// <summary>Field number for the "field" field.</summary>
    public const int FieldFieldNumber = 2;
    private static readonly pb::FieldCodec<Bgs.Protocol.Presence.V1.Field> _repeated_field_codec
        = pb::FieldCodec.ForMessage(18, Bgs.Protocol.Presence.V1.Field.Parser);
    private readonly pbc::RepeatedField<Bgs.Protocol.Presence.V1.Field> field_ = new pbc::RepeatedField<Bgs.Protocol.Presence.V1.Field>();
    public pbc::RepeatedField<Bgs.Protocol.Presence.V1.Field> Field {
      get { return field_; }
    }

    public override bool Equals(object other) {
      return Equals(other as QueryResponse);
    }

    public bool Equals(QueryResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!field_.Equals(other.field_)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      hash ^= field_.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      field_.WriteTo(output, _repeated_field_codec);
    }

    public int CalculateSize() {
      int size = 0;
      size += field_.CalculateSize(_repeated_field_codec);
      return size;
    }

    public void MergeFrom(QueryResponse other) {
      if (other == null) {
        return;
      }
      field_.Add(other.field_);
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 18: {
            field_.AddEntriesFrom(input, _repeated_field_codec);
            break;
          }
        }
      }
    }

  }

  [System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class OwnershipRequest : pb::IMessage<OwnershipRequest> {
    private static readonly pb::MessageParser<OwnershipRequest> _parser = new pb::MessageParser<OwnershipRequest>(() => new OwnershipRequest());
    public static pb::MessageParser<OwnershipRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return Bgs.Protocol.Presence.V1.PresenceServiceReflection.Descriptor.MessageTypes[6]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public OwnershipRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public OwnershipRequest(OwnershipRequest other) : this() {
      EntityId = other.entityId_ != null ? other.EntityId.Clone() : null;
      releaseOwnership_ = other.releaseOwnership_;
    }

    public OwnershipRequest Clone() {
      return new OwnershipRequest(this);
    }

    /// <summary>Field number for the "entity_id" field.</summary>
    public const int EntityIdFieldNumber = 1;
    private Bgs.Protocol.EntityId entityId_;
    public Bgs.Protocol.EntityId EntityId {
      get { return entityId_; }
      set {
        entityId_ = value;
      }
    }

    /// <summary>Field number for the "release_ownership" field.</summary>
    public const int ReleaseOwnershipFieldNumber = 2;
    private bool releaseOwnership_;
    public bool ReleaseOwnership {
      get { return releaseOwnership_; }
      set {
        releaseOwnership_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as OwnershipRequest);
    }

    public bool Equals(OwnershipRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(EntityId, other.EntityId)) return false;
      if (ReleaseOwnership != other.ReleaseOwnership) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (entityId_ != null) hash ^= EntityId.GetHashCode();
      if (ReleaseOwnership != false) hash ^= ReleaseOwnership.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (entityId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(EntityId);
      }
      if (ReleaseOwnership != false) {
        output.WriteRawTag(16);
        output.WriteBool(ReleaseOwnership);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (entityId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EntityId);
      }
      if (ReleaseOwnership != false) {
        size += 1 + 1;
      }
      return size;
    }

    public void MergeFrom(OwnershipRequest other) {
      if (other == null) {
        return;
      }
      if (other.entityId_ != null) {
        if (entityId_ == null) {
          entityId_ = new Bgs.Protocol.EntityId();
        }
        EntityId.MergeFrom(other.EntityId);
      }
      if (other.ReleaseOwnership != false) {
        ReleaseOwnership = other.ReleaseOwnership;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (entityId_ == null) {
              entityId_ = new Bgs.Protocol.EntityId();
            }
            input.ReadMessage(entityId_);
            break;
          }
          case 16: {
            ReleaseOwnership = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
